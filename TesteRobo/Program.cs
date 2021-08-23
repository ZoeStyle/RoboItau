using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;

namespace TesteRobo
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlDadosConta = File.ReadAllText(@"D:\Projects\Pessoais\TesteRobo\TesteRobo\DadosConta.ini");

            string agencia = ColetarInformacoesConta(xmlDadosConta, "<agencia>(.*?)</agencia>");
            string conta = ColetarInformacoesConta(xmlDadosConta, "<conta>(.*?)</conta>");
            string senha = ColetarInformacoesConta(xmlDadosConta, "<senha>(.*?)</senha>");

            if(string.IsNullOrEmpty(agencia) || string.IsNullOrEmpty(conta) || string.IsNullOrEmpty(senha))
            {
                Console.WriteLine("Voce deve preencher os campos da xonta no xml");
                return;
            }

            RoboItau(agencia, conta, senha);

            Console.WriteLine("Obrigado por utilizar o robo do itaú");
            Console.ReadKey();
        }

        public static void RoboItau(string agencia, string conta, string senha)
        {
            IWebDriver driver = new FirefoxDriver(@"D:\Projects\Pessoais\TesteRobo\TesteRobo");

            driver.Manage().Window.Maximize();
            driver.Url = "http://bankline.com.br/";

            //Envia os dados da agencia
            EnviarDados(driver, "//*[@id='agencia']", agencia);

            //Envia os dados da conta
            EnviarDados(driver, "//*[@id='conta']", conta);

            EventoOnClinck(driver, "//*[@id='btnLoginSubmit']");

            Thread.Sleep(7000);

            var elemetos = ListaDeDadosDeCadaElemento(driver);

            DigitarSenha(driver, elemetos, senha);

            EventoOnClinck(driver, "//*[@id='acessar']");

            Thread.Sleep(5000);

            EnviarDados(driver, "//*[@id='VerExtrato']", Keys.Return);

            Thread.Sleep(25000);
            driver.Close();
        }

        public static void EventoOnClinck(IWebDriver driver, string xpath)
        {
            driver.FindElement(By.XPath(xpath)).Click();
        }

        public static void EnviarDados(IWebDriver driver, string xpath, string value)
        {

            driver.FindElement(By.XPath(xpath)).SendKeys(value);
        }

        public static IList<string> ListaDeDadosDeCadaElemento(IWebDriver driver)
        {
            IList<string> list = new List<string>();

            for (var i = 1; i <= 5; i++)
            {
                list.Add(driver.FindElement(By.XPath("//*[@id='frmKey']/fieldset/div[2]/div[1]/a[" + i + "]")).Text);
            }
            return list;
        }

        public static void DigitarSenha(IWebDriver driver, IList<string> elementos, string senha)
        {
            foreach (var digito in senha)
            {
                ValidarDigitosSenha(driver, elementos, digito);
            }
        }


        public static void ValidarDigitosSenha(IWebDriver driver, IList<string> elementos, char digito)
        {
            for (var i = 0; i <= elementos.Count; i++)
            {
                if (elementos[i].Contains(digito))
                {
                    driver.FindElement(By.XPath("//*[@id='frmKey']/fieldset/div[2]/div[1]/a[" + (i + 1) + "]")).Click();
                    break;
                }
            }
        }

        public static string ColetarInformacoesConta(string mensagem, string valorDesejado)
        {
            try
            {
                Match match = Regex.Match(mensagem, valorDesejado);
                    return match.Groups[1].Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
