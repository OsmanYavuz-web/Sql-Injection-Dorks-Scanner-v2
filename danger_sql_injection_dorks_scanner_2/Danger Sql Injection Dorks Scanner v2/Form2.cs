using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Danger_Sql_Injection_Dorks_Scanner_v2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        #region Guncelleme
        private void guncelleme()
        {
            try
            {
                #region Sürüm bul
                Uri url = new Uri("http://hatosbilisim.com/programlar");
                WebClient client = new WebClient() { Encoding = Encoding.UTF8 };
                string html = client.DownloadString(url);
                HtmlAgilityPack.HtmlDocument dokuman = new HtmlAgilityPack.HtmlDocument();
                dokuman.LoadHtml(html);

                string gelen = html;
                int titleIndexBaslangici = gelen.IndexOf(textBox1.Text) + textBox1.TextLength;
                int titleIndexBitisi = gelen.Substring(titleIndexBaslangici).IndexOf(textBox3.Text);
                string gelen2 = gelen.Substring(titleIndexBaslangici, titleIndexBitisi);
                int titleIndexBaslangici2 = gelen2.IndexOf("<li><b>Sürüm: </b>") + 18;
                int titleIndexBitisi2 = gelen2.Substring(titleIndexBaslangici2).IndexOf("</li>");
                richTextBox4.Text = gelen2.Substring(titleIndexBaslangici2, titleIndexBitisi2);
                #endregion

                INI INI = new INI(Application.StartupPath + @"\ayarlar.ini");
                #region Sürüm karşılaştır
                string mevcutSurum = INI.Read("Ayarlar", "Surum");
                string bulunanSurum = richTextBox4.Text;
                if (bulunanSurum == mevcutSurum)
                {
                    MessageBox.Show("Program en güncel sürümde.","Bilgi",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                else
                {
                    DialogResult soru = MessageBox.Show("Yeni güncelleştirme bulundu!\nSizin sürümünüz: " + mevcutSurum + " - Güncel sürüm: " + bulunanSurum + " \n\nGüncelleştirmek istiyorsanız 'Evet' butonuna tıklayarak indirme adresine gidebilirsiniz.", "Güncelleştirme;", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (soru == DialogResult.Yes)
                    {
                        string gelen3 = gelen.Substring(titleIndexBaslangici, titleIndexBitisi);
                        int titleIndexBaslangici3 = gelen3.IndexOf(textBox6.Text) + textBox6.TextLength;
                        int titleIndexBitisi3 = gelen3.Substring(titleIndexBaslangici3).IndexOf(textBox5.Text);
                        string link = gelen3.Substring(titleIndexBaslangici3, titleIndexBitisi3);
                        string key = @"http\shell\open\command";
                        RegistryKey registryKey =
                        Registry.ClassesRoot.OpenSubKey(key, false);
                        string defaultbrowserpath =
                        ((string)registryKey.GetValue(null, null)).Split('"')[1];
                        Process.Start(defaultbrowserpath, link);
                    }
                }
                #endregion
            }
            catch { }
        }
        #endregion

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            guncelleme();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
           //Guncelleme

           INI INI = new INI(Application.StartupPath + @"\ayarlar.ini");
           textBox50.Text = INI.Read("Ayarlar", "Surum");
        }
    }
}
