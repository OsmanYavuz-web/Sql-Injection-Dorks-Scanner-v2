using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//Eklenenler
using System.IO;
using HtmlAgilityPack;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Diagnostics;

namespace Danger_Sql_Injection_Dorks_Scanner_v2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string hedefUlkeUzantisi = null;
        DateTime ilkZaman = DateTime.Now;
        DateTime sonZaman = DateTime.Now;
        string programYol = Application.StartupPath;
        INI INI = new INI(Application.StartupPath + @"\ayarlar.ini");
        string yol = null;
        string klasorAdi = null;
        string hedefYol = null;
        Thread tZafiyetTaramasi;
        Thread islem;

        #region FORM_LOAD
        private void Form1_Load(object sender, EventArgs e)
        {
            //Guncelleme
            INI.Write("Ayarlar", "Surum", "2.0");
            guncelleme();

            //Form Boyutu
            this.Size = new Size(813, 664);

            //Txt Dosyaları Aktar
            dosyaAktarmaIslemleri();

            //Proxy Listesi
            proxyList();

            //Thread Çalıştırma
            CheckForIllegalCrossThreadCalls = false;
        }
        #endregion

        #region Dosya Aktarma İşlemleri
        private void dosyaAktarmaIslemleri()
        {
            INI INI = new INI(programYol + @"\ayarlar.ini");

            //Max Site
            textBox_MaxSite.Text = INI.Read("Ayarlar", "MaxSite");
            //Max Sure
            textBox_MaxSure.Text = INI.Read("Ayarlar", "MaxSure");

            #region Admin Panels
            try
            {
                if (File.Exists(programYol + @"\txt\admin-panels.txt"))
                {
                    //Yaz
                    INI.Write("Ayarlar", "AdminPanelsYol", programYol + @"\txt\admin-panels.txt");
                    //Oku
                    textBox_AdminPanelsDosyasi.Text = INI.Read("Ayarlar", "AdminPanelsYol");
                    StreamReader oku;
                    oku = File.OpenText(textBox_AdminPanelsDosyasi.Text);
                    string yaz;
                    while ((yaz = oku.ReadLine()) != null)
                    {
                        listBox_AdminPanels.Items.Add(yaz.ToString());
                    }
                    oku.Close();
                }
                else
                {
                    MessageBox.Show("admin-panels.txt Dosyası bulunamadı! \n'Ayarlar/Diğer Ayarlar' kısmından belirtmeniz gerekmekte.", "Hata;", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
            #endregion

            #region Ülke Uzantıları
            try
            {
                if (File.Exists(programYol + @"\txt\ulke-uzantilari.txt"))
                {
                    //Yaz
                    INI.Write("Ayarlar", "UlkeUzantiYol", programYol + @"\txt\ulke-uzantilari.txt");
                    //Oku
                    textBox_UlkeUzantilariDosyasi.Text = INI.Read("Ayarlar", "UlkeUzantiYol");
                    StreamReader oku;
                    oku = File.OpenText(textBox_UlkeUzantilariDosyasi.Text);
                    string yaz;
                    while ((yaz = oku.ReadLine()) != null)
                    {
                        comboBox_UlkeUzantilari.Items.Add(yaz.ToString());
                    }
                    oku.Close();
                }
                else
                {
                    MessageBox.Show("ulke-uzantilari.txt Dosyası bulunamadı! \n'Ayarlar/Diğer Ayarlar' kısmından belirtmeniz gerekmekte.", "Hata;", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }

            #endregion

            #region Test Dorks
            try
            {
                if (File.Exists(programYol + @"\txt\test-dorks.txt"))
                {
                    //Oku
                    textBox_TestDorksDosyasi.Text = INI.Read("Ayarlar", "TestDorksYol");

                    if (INI.Read("Ayarlar", "TestDorksAktif") == "Evet")
                    {
                        checkBox_TestDorksAktiflestir.Checked = true;
                        //Yaz
                        INI.Write("Ayarlar", "TestDorksYol", programYol + @"\txt\test-dorks.txt");
                        //Oku
                        textBox_TestDorksDosyasi.Text = INI.Read("Ayarlar", "TestDorksYol");
                    }
                    else
                    {
                        checkBox_TestDorksAktiflestir.Checked = false;
                    }
                }
                else
                {
                    MessageBox.Show("test-dorks.txt Dosyası bulunamadı! \n'Ayarlar/Diğer Ayarlar' kısmından belirtmeniz gerekmekte.", "Hata;", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            catch { }
            #endregion

            #region User Agent
            try
            {
                if (File.Exists(programYol + @"\txt\user-agents.txt"))
                {
                    //Oku
                    textBox_UserAgentDosyasi.Text = INI.Read("Ayarlar", "UserAgentYol");

                    if (INI.Read("Ayarlar", "UserAgentAktif") == "Evet")
                    {
                        checkBox_UserAgentAktif.Checked = true;
                        //Yaz
                        INI.Write("Ayarlar", "UserAgentYol", programYol + @"\txt\user-agents.txt");
                        //Oku
                        textBox_UserAgentDosyasi.Text = INI.Read("Ayarlar", "TestDorksYol");

                    }
                    else
                    {
                        checkBox_UserAgentAktif.Checked = false;
                    }
                }
                else
                {
                    MessageBox.Show("user-agent.txt Dosyası bulunamadı! \n'Ayarlar/UserAgent & Proxy Ayarları' kısmından belirtmeniz gerekmekte.", "Hata;", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
            #endregion

            #region Proxy
            if (INI.Read("Ayarlar", "ProxyAktif") == "Evet")
            {
                textBox_SeciliProxy.Text = INI.Read("Ayarlar", "ProxyAdres") + ":"+ INI.Read("Ayarlar", "ProxyPort");
                checkBox_ProxyAktiflestir.Checked = true;
            }
            #endregion
        }
        #endregion

        #region Dork List Seç
        private void button_DorkSec_Click(object sender, EventArgs e)
        {
            openFileDialog_DorkList.FileName = "";
            openFileDialog_DorkList.Filter = "Txt Dosyası(.txt) |*.txt";
            DialogResult dosya = openFileDialog_DorkList.ShowDialog();
            if (dosya == DialogResult.OK)
            {
                StreamReader oku;
                oku = File.OpenText(openFileDialog_DorkList.FileName);
                string yaz;
                while ((yaz = oku.ReadLine()) != null)
                {
                    yaz = yaz.Replace("inurl:","");
                    listBox_DorkList.Items.Add(yaz.ToString());
                }
                oku.Close();

                if(listBox_DorkList.Items.IndexOf("Son") != -1)
                {
                    listBox_DorkList.Items.Remove("Son");
                    listBox_DorkList.Items.Add("Son");
                }
                else
                {
                    listBox_DorkList.Items.Add("Son");
                }

                if (listBox_DorkList.Items.Count > 1)
                {
                    MessageBox.Show("Toplam " + (listBox_DorkList.Items.Count - 1).ToString() + " dork bulundu ve aktarıldı.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    button_TaramayiBaslat.Enabled = true;
                    comboBox_UlkeUzantilari.Enabled = true;
                }
                else
                {
                    listBox_DorkList.Items.Clear();
                    MessageBox.Show("Ops! Toplam 1 dork bulundu. \nBiliyorum çok saçma bir hata oldu ama en az dork sayısı 2 olmalı :)", "Hata;", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Herhangi bir dosya seçmediniz.","Bilgi;",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Dork List Temizle
        private void button_DorkListTemizle_Click(object sender, EventArgs e)
        {
            DialogResult soru = MessageBox.Show("Toplam " + (listBox_DorkList.Items.Count - 1).ToString() + " dorku silmek istediğinize emin misiniz?", "Silme işlemi;", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (soru == DialogResult.Yes)
            {
                listBox_DorkList.Items.Clear();
                button_TaramayiBaslat.Enabled = false;
                button_TaramayiDurdur.Enabled = false;
            }
        }
        #endregion

        #region Bulunan Site Seçili
        private void listBox_BulunanLinkler_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox_BulunanLinkSecili.Text = listBox_BulunanLinkler.Text;
        }
        private void listBox_AcikBulunanSiteler_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox_AcikBulunanSiteSecili.Text = listBox_AcikBulunanSiteler.Text;
            //Seçili Hedef Admin Panel için
            string gelen = listBox_AcikBulunanSiteler.Text;
            int titleIndexBaslangici = gelen.IndexOf("http://") + 7;
            int titleIndexBitisi = gelen.Substring(titleIndexBaslangici).IndexOf("/");
           textBox_AdminPanelSeciliSite.Text = "http://" + gelen.Substring(titleIndexBaslangici, titleIndexBitisi) + "/";
        }
        #endregion

        #region Geçen Zaman
        private void gecen_Zaman()
        {
            int saniye = ilkZaman.Second;
            int dakika = ilkZaman.Minute;

            int saniye2 = sonZaman.Second;
            int dakika2 = sonZaman.Minute;

            int sonuc = saniye2 - saniye;
            int sonuc2 = dakika2 - dakika;

            if (sonuc2 == 0) 
            {
                label_GecenZaman.Text = "Geçen Zaman: " + sonuc2.ToString() + " DK " + sonuc.ToString() + " SN".Replace("-","");
            }
            else
            {
                label_GecenZaman.Text = "Geçen Zaman: " + sonuc2.ToString() + " DK " + sonuc.ToString() + " SN".Replace("-", "");
            }
            label_GecenZaman.Text = label_GecenZaman.Text.Replace("-","");
        }
        #endregion

        #region Proxy List Çek
        private void proxyList()
        {
            try
            {
                listBox_Proxy.Items.Clear();
                listBox_Ulke.Items.Clear();
                listBox_Hızı.Items.Clear();

                Uri url = new Uri("http://proxy-list.org/english/index.php?p=1");
                WebClient client = new WebClient() { Encoding = Encoding.UTF8 };
                string html = client.DownloadString(url);
                HtmlAgilityPack.HtmlDocument dokuman = new HtmlAgilityPack.HtmlDocument();
                dokuman.LoadHtml(html);

                HtmlNodeCollection XPath = dokuman.DocumentNode.SelectNodes("//*[@id='proxy-table']/div[2]/div");
                foreach (var veri in XPath)
                {
                    richTextBox1.Text = veri.InnerHtml;
                }

                HtmlAgilityPack.HtmlDocument dokuman2 = new HtmlAgilityPack.HtmlDocument();
                dokuman2.LoadHtml(richTextBox1.Text);
                HtmlNodeCollection XPath2 = dokuman2.DocumentNode.SelectNodes("//li[@class='proxy']");
                foreach (var veri2 in XPath2)
                {
                    listBox_Proxy.Items.Add(veri2.InnerText);
                }
                HtmlNodeCollection XPath3 = dokuman2.DocumentNode.SelectNodes("//span[@class='name']");
                foreach (var veri3 in XPath3)
                {
                    listBox_Ulke.Items.Add(veri3.InnerText);
                }
                HtmlNodeCollection XPath4 = dokuman2.DocumentNode.SelectNodes("//li[@class='speed']");
                foreach (var veri4 in XPath4)
                {
                    listBox_Hızı.Items.Add(veri4.InnerText);
                }
            }
            catch { }
        }

        private void listBox_Proxy_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_Ulke.SelectedIndex = listBox_Proxy.SelectedIndex;
            listBox_Hızı.SelectedIndex = listBox_Proxy.SelectedIndex;
            textBox_SeciliProxy.Text = listBox_Proxy.Text;
        }
        private void listBox_Ulke_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_Proxy.SelectedIndex = listBox_Ulke.SelectedIndex;
            listBox_Hızı.SelectedIndex = listBox_Ulke.SelectedIndex;

        }
        private void listBox_Hızı_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_Ulke.SelectedIndex = listBox_Hızı.SelectedIndex;
            listBox_Proxy.SelectedIndex = listBox_Hızı.SelectedIndex;
        }
        int sure = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            sure = sure + 1;
            if (sure == 62)
            {
                if (INI.Read("Ayarlar", "ProxyAktif") == "Evet")
                {
                    proxyList();
                }
                sure = 1;
            }
        }
        #endregion

        #region Ayarlar

        #region Test Dorks Aktifleştir
        private void checkBox_TestDorksAktiflestir_CheckedChanged(object sender, EventArgs e)
        {
            //Ayarlar.ini Dosyası
            INI INI = new INI(programYol + @"\ayarlar.ini");

            if (checkBox_TestDorksAktiflestir.Checked == true)
            {
                //Yaz
                INI.Write("Ayarlar", "TestDorksAktif", "Evet");

                DialogResult soru = MessageBox.Show("test-dorks.txt Dosyası dork listesine aktarılsın mı?", "Bilgi;", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (soru == DialogResult.Yes)
                {
                    //Yaz
                    INI.Write("Ayarlar", "TestDorksYol", programYol + @"\txt\test-dorks.txt");
                    //Oku
                    textBox_TestDorksDosyasi.Text = INI.Read("Ayarlar", "TestDorksYol");
                    StreamReader oku;
                    oku = File.OpenText(textBox_TestDorksDosyasi.Text);
                    string yaz;
                    while ((yaz = oku.ReadLine()) != null)
                    {
                        yaz = yaz.Replace("inurl:", "");
                        yaz = yaz.Replace("/", "");
                        listBox_DorkList.Items.Add(yaz.ToString());
                    }
                    oku.Close();

                    if (listBox_DorkList.Items.IndexOf("Son") != -1)
                    {
                        listBox_DorkList.Items.Remove("Son");
                        listBox_DorkList.Items.Add("Son");
                    }
                    else
                    {
                        listBox_DorkList.Items.Add("Son");
                    }

                    button_TaramayiBaslat.Enabled = true;
                    comboBox_UlkeUzantilari.Enabled = true;
                    MessageBox.Show("test-dorks.txt Dosyası başarılı bir şekilde aktarıldı. \nToplam dork: " + (listBox_DorkList.Items.Count - 1).ToString(), "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (checkBox_TestDorksAktiflestir.Checked == false)
            {
                //Yaz
                INI.Write("Ayarlar", "TestDorksAktif", "Hayır");
                checkBox_TestDorksAktiflestir.Checked = false;
            }
        }
        #endregion

        #region Admin Panels Değiştir
        private void button__AdminPanelsDosyasiSec_Click(object sender, EventArgs e)
        {
            //Ayarlar.ini Dosyası
            INI INI = new INI(programYol + @"\ayarlar.ini");
            openFileDialog_AdminPanelsDosyasiSec.FileName = "";
            openFileDialog_AdminPanelsDosyasiSec.Filter = "Txt Dosyası(.txt) |*.txt";
            DialogResult dosya = openFileDialog_AdminPanelsDosyasiSec.ShowDialog();
            if (dosya == DialogResult.OK)
            {
                DialogResult soru = MessageBox.Show(openFileDialog_AdminPanelsDosyasiSec.SafeFileName + " Dosyası admin panels listesine aktarılsın mı?", "Bilgi;", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (soru == DialogResult.Yes)
                {
                    StreamReader oku;
                    oku = File.OpenText(openFileDialog_AdminPanelsDosyasiSec.FileName);
                    string yaz;
                    while ((yaz = oku.ReadLine()) != null)
                    {
                        listBox_AdminPanels.Items.Add(yaz.ToString());
                    }
                    oku.Close();
                    MessageBox.Show(openFileDialog_AdminPanelsDosyasiSec.SafeFileName + " Dosyası başarılı bir şekilde aktarıldı. \nToplam veri: " + listBox_AdminPanels.Items.Count.ToString(), "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(openFileDialog_AdminPanelsDosyasiSec.SafeFileName + " Dosyası aktarılmadı. Programı tekrar başlattığınızda aktarılmış olacaktır.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                //Yaz
                INI.Write("Ayarlar", "AdminPanelsYol", openFileDialog_AdminPanelsDosyasiSec.FileName);
                textBox_AdminPanelsDosyasi.Text = INI.Read("Ayarlar", "AdminPanelsYol");
            }
            else
            {
                MessageBox.Show("Herhangi bir dosya seçmediniz.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region User Agent Aktifleştir
        private void checkBox_UserAgentAktif_CheckedChanged(object sender, EventArgs e)
        {
            //Ayarlar.ini Dosyası
            INI INI = new INI(programYol + @"\ayarlar.ini");

            if (checkBox_UserAgentAktif.Checked == true)
            {
                //Yaz
                INI.Write("Ayarlar", "UserAgentAktif", "Evet");

                //Yaz
                INI.Write("Ayarlar", "UserAgentYol", programYol + @"\txt\user-agents.txt");
                //Oku
                textBox_TestDorksDosyasi.Text = INI.Read("Ayarlar", "UserAgentYol");

                StreamReader oku;
                oku = File.OpenText(textBox_UserAgentDosyasi.Text);
                string yaz;
                while ((yaz = oku.ReadLine()) != null)
                {
                    listBox_UserAgentList.Items.Add(yaz.ToString());
                }
                oku.Close();
            }
            else if (checkBox_UserAgentAktif.Checked == false)
            {
                //Yaz
                INI.Write("Ayarlar", "UserAgentAktif", "Hayır");
                checkBox_UserAgentAktif.Checked = false;
            }
        }
        #endregion

        #region Max Site
        private void textBox_MaxSite_TextChanged(object sender, EventArgs e)
        {
            int max = int.Parse(textBox_MaxSite.Text);
            if (max > 100)
            {
                MessageBox.Show("Google arama motoru bir sayfada en fazla 100 url listeleyebilir.");
                textBox_MaxSite.Text = "100";
            }
            else
            {
                //Ayarlar.ini Dosyası
                INI INI = new INI(programYol + @"\ayarlar.ini");
                //Yaz
                INI.Write("Ayarlar", "MaxSite", textBox_MaxSite.Text);
            }
        }
        private void textBox_MaxSite_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        #endregion

        #region Max Sure
        private void textBox_MaxSure_TextChanged(object sender, EventArgs e)
        {
            //Ayarlar.ini Dosyası
            INI INI = new INI(programYol + @"\ayarlar.ini");
            //Yaz
            INI.Write("Ayarlar", "MaxSure", textBox_MaxSure.Text);
        }
        private void textBox_MaxSure_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

        }
        #endregion

        #region Proxy Kaydet
        private void checkBox_ProxyAktiflestir_CheckedChanged(object sender, EventArgs e)
        {
            //Ayarlar.ini Dosyası
            INI INI = new INI(programYol + @"\ayarlar.ini");

            if (checkBox_ProxyAktiflestir.Checked == true)
            {
                //Yaz
                INI.Write("Ayarlar", "ProxyAktif", "Evet");
                groupBox_Proxy.Enabled = true;
            }
            else if (checkBox_ProxyAktiflestir.Checked == false)
            {
                //Yaz
                INI.Write("Ayarlar", "ProxyAktif", "Hayır");
                INI.Write("Ayarlar", "ProxyAdres", "null");
                INI.Write("Ayarlar", "ProxyPort", "null");
                checkBox_ProxyAktiflestir.Checked = false;
                groupBox_Proxy.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Ayarlar.ini Dosyası
            INI INI = new INI(programYol + @"\ayarlar.ini");

            if (checkBox_ProxyAktiflestir.Checked == true)
            {
                //Yaz
                INI.Write("Ayarlar", "ProxyAktif", "Evet");
                if (textBox_SeciliProxy.Text == "")
                {
                    MessageBox.Show("Proxy'i uygulamak için ilk önce listeden proxy seçmelisiniz.", "Hata;", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    

                    string[] parcalar;
                    parcalar = textBox_SeciliProxy.Text.Split(':');
                    string adress = parcalar[0];
                    string port  = parcalar[1];

                    //Yaz
                    INI.Write("Ayarlar", "ProxyAktif", "Evet");
                    INI.Write("Ayarlar", "ProxyAdres", adress);
                    INI.Write("Ayarlar", "ProxyPort", port);
                    MessageBox.Show(adress + ":" + port + " Seçili proxy başarıyla uygulandı.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (checkBox_ProxyAktiflestir.Checked == false)
            {
                MessageBox.Show("Seçili proxy'i uygulamak için proxy'i aktifleştirmeniz gerek.","Hata;",MessageBoxButtons.OK,MessageBoxIcon.Error);
                //Yaz
                INI.Write("Ayarlar", "ProxyAktif", "Hayır");
                INI.Write("Ayarlar", "ProxyAdres", "null");
                INI.Write("Ayarlar", "ProxyPort", "null");
                checkBox_ProxyAktiflestir.Checked = false;
            }
        }
        #endregion

        private void comboBox_UlkeUzantilari_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_UlkeUzantilari.Text.IndexOf(".tr") != -1)
            {
                MessageBox.Show("Türkiye uzantısını seçtin. Ne yaptığını biliyorsundur umarım? :)", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void comboBox_UlkeUzantilari_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = Char.IsLetterOrDigit(e.KeyChar) || Char.IsSymbol(e.KeyChar) || Char.IsPunctuation(e.KeyChar) || Char.IsWhiteSpace(e.KeyChar) || Char.IsControl(e.KeyChar) || Char.IsNumber(e.KeyChar);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox_AcikBulunanSiteSecili.Text == "") { }
                else
                {
                    string key = @"http\shell\open\command";
                    RegistryKey registryKey =
                    Registry.ClassesRoot.OpenSubKey(key, false);
                    string defaultbrowserpath =
                    ((string)registryKey.GetValue(null, null)).Split('"')[1];
                    Process.Start(defaultbrowserpath, textBox_AcikBulunanSiteSecili.Text);
                }
            }
            catch { }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox_BulunanLinkSecili.Text == "") { }
                else
                {
                    string key = @"http\shell\open\command";
                    RegistryKey registryKey =
                    Registry.ClassesRoot.OpenSubKey(key, false);
                    string defaultbrowserpath =
                    ((string)registryKey.GetValue(null, null)).Split('"')[1];
                    Process.Start(defaultbrowserpath, textBox_BulunanLinkSecili.Text);
                }
            }
            catch { }
        }
        #endregion

        #region Menüler

        #region Program Hakkında
        private void programHakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.Show();
        }
        #endregion

        #region Bulunan Siteler
        private void bulunanSiteleriKaydetToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //Klasor Seç
            folderBrowserDialog1.ShowNewFolderButton = true;
            // Kontrolü göster
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                //Dosyayı açmaya çalış yoksa catch kısmına geç
                try
                {
                    StreamReader Dosya = File.OpenText(folderBrowserDialog1.SelectedPath + @"\BulunanSiteler.txt");
                    Dosya.Close();
                    File.Delete(folderBrowserDialog1.SelectedPath + @"\BulunanSiteler.txt");
                    try
                    {
                        //Dosyayı appendText ile yazmak için açtık
                        StreamWriter dosyaAc = File.AppendText(folderBrowserDialog1.SelectedPath + @"\BulunanSiteler.txt");
                        // Dosya.WriteLine ile dosyaya verileri ekledik.
                        dosyaAc.WriteLine("## Dork Taramasında Bulunan Siteler ##");
                        dosyaAc.WriteLine("Kayıt Tarihi= " + DateTime.Now);
                        dosyaAc.WriteLine("Toplam Site = " + textBox_ToplamLink.Text + "\n");

                        foreach (var item in listBox_BulunanLinkler.Items)
                        {
                            dosyaAc.WriteLine(item);
                        }
                        // Dosya yı kapattık.
                        dosyaAc.Close();
                        MessageBox.Show("Bulunan site sonuçları seçtiğiniz dizine kaydedildi.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch { }
                }
                catch
                {
                    try
                    {
                        //Dosyayı appendText ile yazmak için açtık
                        StreamWriter dosyaAc = File.AppendText(folderBrowserDialog1.SelectedPath + @"\BulunanSiteler.txt");
                        // Dosya.WriteLine ile dosyaya verileri ekledik.
                        dosyaAc.WriteLine("## ReverseIP Adresleri ##");
                        dosyaAc.WriteLine("Kayıt Tarihi= " + DateTime.Now);
                        dosyaAc.WriteLine("Toplam Site = " + textBox_ToplamLink.Text + "\n");

                        foreach (var item in listBox_BulunanLinkler.Items)
                        {
                            dosyaAc.WriteLine(item);
                        }
                        // Dosya yı kapattık.
                        dosyaAc.Close();
                        MessageBox.Show("Bulunan site sonuçları seçtiğiniz dizine kaydedildi.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch { }
                }
            }
            else
            {
                MessageBox.Show("Kaydedilecek klasörü seçmediniz.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Zafiyet Bulunan Siteler
        private void zafiyetBulunanSiteleriKaydetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Klasor Seç
            folderBrowserDialog2.ShowNewFolderButton = true;
            // Kontrolü göster
            DialogResult result = folderBrowserDialog2.ShowDialog();
            if (result == DialogResult.OK)
            {
                //Dosyayı açmaya çalış yoksa catch kısmına geç
                try
                {
                    StreamReader Dosya = File.OpenText(folderBrowserDialog2.SelectedPath + @"\ZafiyetBulunanSiteler.txt");
                    Dosya.Close();
                    File.Delete(folderBrowserDialog2.SelectedPath + @"\ZafiyetBulunanSiteler.txt");
                    try
                    {
                        //Dosyayı appendText ile yazmak için açtık
                        StreamWriter dosyaAc = File.AppendText(folderBrowserDialog2.SelectedPath + @"\ZafiyetBulunanSiteler.txt");
                        // Dosya.WriteLine ile dosyaya verileri ekledik.
                        dosyaAc.WriteLine("## Zafiyet Bulunan Siteler ##");
                        dosyaAc.WriteLine("Kayıt Tarihi= " + DateTime.Now);
                        dosyaAc.WriteLine("Toplam Site = " + listBox_AcikBulunanSiteler.Items.Count.ToString() + "\n");

                        foreach (var item in listBox_AcikBulunanSiteler.Items)
                        {
                            dosyaAc.WriteLine(item);
                        }
                        // Dosya yı kapattık.
                        dosyaAc.Close();
                        MessageBox.Show("Zafiyet bulunan site sonuçlarını seçtiğiniz dizine kaydedildi.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch { }
                }
                catch
                {
                    try
                    {
                        //Dosyayı appendText ile yazmak için açtık
                        StreamWriter dosyaAc = File.AppendText(folderBrowserDialog2.SelectedPath + @"\ZafiyetBulunanSiteler.txt");
                        // Dosya.WriteLine ile dosyaya verileri ekledik.
                        dosyaAc.WriteLine("## ReverseIP Adresleri ##");
                        dosyaAc.WriteLine("Kayıt Tarihi= " + DateTime.Now);
                        dosyaAc.WriteLine("Toplam Site = " + listBox_AcikBulunanSiteler.Items.Count.ToString() + "\n");

                        foreach (var item in listBox_AcikBulunanSiteler.Items)
                        {
                            dosyaAc.WriteLine(item);
                        }
                        // Dosya yı kapattık.
                        dosyaAc.Close();
                        MessageBox.Show("Zafiyet bulunan site sonuçlarını seçtiğiniz dizine kaydedildi.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch { }
                }
            }
            else
            {
                MessageBox.Show("Kaydedilecek klasörü seçmediniz.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Program Kapatma İşlemleri
        private void programıKapatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult soru = MessageBox.Show("Program kapatılsın mı?", "İşlem;", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (soru == DialogResult.Yes)
            {
                try
                {
                    islem.Abort();
                    tZafiyetTaramasi.Abort();
                    Proxy.VarsayılanProxy();
                    Application.Exit();
                }
                catch
                {
                    Proxy.VarsayılanProxy();
                    Application.Exit();
                }
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                islem.Abort();
                tZafiyetTaramasi.Abort();
                Proxy.VarsayılanProxy();
                Application.Exit();
            }
            catch
            {
                Proxy.VarsayılanProxy();
                Application.Exit();
            }

        }
        private void button_ProgramiKapat_Click(object sender, EventArgs e)
        {
            DialogResult soru = MessageBox.Show("Program kapatılsın mı?","İşlem;",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (soru == DialogResult.Yes)
            {
                try
                {
                    islem.Abort();
                    tZafiyetTaramasi.Abort();
                    Proxy.VarsayılanProxy();
                    Application.Exit();
                }
                catch
                {
                    Proxy.VarsayılanProxy();
                    Application.Exit();
                }
            }
        }
        #endregion

        #region Dışarıdan Site Listesi Yükle
        private void dışarıdaSiteListesiYükleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Txt Dosyası(.txt) |*.txt";
            DialogResult dosya = openFileDialog1.ShowDialog();
            if (dosya == DialogResult.OK)
            {
                StreamReader oku;
                oku = File.OpenText(openFileDialog1.FileName);
                string yaz;
                while ((yaz = oku.ReadLine()) != null)
                {
                    listBox_BulunanLinkler.Items.Add(yaz);
                }
                oku.Close();
                listBox_BulunanLinkler.Items.Remove(0);
                listBox_BulunanLinkler.Items.Remove(1);
                listBox_BulunanLinkler.Items.Remove(2);
                listBox_BulunanLinkler.Items.Remove(3);

                textBox_ToplamLink.Text = listBox_BulunanLinkler.Items.Count.ToString();
                if (listBox_BulunanLinkler.Items.Count > 0)
                {
                    DialogResult soru = MessageBox.Show("Toplam: [" + listBox_BulunanLinkler.Items.Count.ToString() + "] site bulundu.", "İşlem;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Herhangi bir dosya seçmediniz.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #endregion

        #region Sonuçları Kaydet
        private void button_SonuclariKaydet_Click(object sender, EventArgs e)
        {
            //Klasor Seç
            folderBrowserDialog3.ShowNewFolderButton = true;
            // Kontrolü göster
            DialogResult result = folderBrowserDialog3.ShowDialog();

            if (result == DialogResult.OK)
            {
                yol = folderBrowserDialog3.SelectedPath;

                try
                {
                    //Klasörün adını hedef site adı yapıyoruz.
                    DateTime dt = DateTime.Now;
                    klasorAdi = "["+ String.Format("{0:d.M.yyyy - HH.mm}", dt) + "] Tarihli Tarama";

                    //Sonuçların kaydedileceği dizin varmı?
                    if (!Directory.Exists(yol + @"\Çıktılar"))
                    {
                        //YOKSA

                        //Çıktılar diye dizin oluştur
                        Directory.CreateDirectory(yol + @"\Çıktılar");

                        //Sonuçların kaydedileceği dizin
                        hedefYol = yol + @"\Çıktılar\" + klasorAdi;

                        //Böyle bir dizin varmı?
                        if (!Directory.Exists(hedefYol))
                        {
                            //YOKSA
                            //Sitenin sonuçlarını kaydedileceği dizini oluştur
                            Directory.CreateDirectory(hedefYol);
                            ciktilariKaydet();
                        }
                        else
                        {
                            //VARSA
                            ciktilariKaydet();
                        }
                    }
                    else
                    {
                        //VARSA

                        //Sonuçların kaydedileceği dizin
                        hedefYol = yol + @"\Çıktılar\" + klasorAdi;
                        //Böyle bir dizin varmı?
                        if (!Directory.Exists(hedefYol))
                        {
                            //YOKSA
                            //Sitenin sonuçlarını kaydedileceği dizini oluştur
                            Directory.CreateDirectory(hedefYol);
                            ciktilariKaydet();
                        }
                        else
                        {
                            //VARSA
                            //Klasörü ve içindekilerin hepsini sil
                            Directory.Delete(hedefYol, true);
                            //Sitenin sonuçlarını kaydedileceği dizini oluştur
                            Directory.CreateDirectory(hedefYol);
                            ciktilariKaydet();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata; " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Kaydedilecek klasörü seçmediniz.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ciktilariKaydet()
        {
            //Dosyayı açmaya çalış yoksa catch kısmına geç
            try
            {
                #region BulunanSiteler.txt
                StreamReader Dosya = File.OpenText(hedefYol + @"\BulunanSiteler.txt");
                Dosya.Close();
                File.Delete(hedefYol + @"\BulunanSiteler.txt");
                try
                {
                    //Dosyayı appendText ile yazmak için açtık
                    StreamWriter dosyaAc = File.AppendText(hedefYol + @"\BulunanSiteler.txt");
                    // Dosya.WriteLine ile dosyaya verileri ekledik.
                    dosyaAc.WriteLine("## Dork Taramasında Bulunan Siteler ##");
                    dosyaAc.WriteLine("Kayıt Tarihi= " + DateTime.Now);
                    dosyaAc.WriteLine("Toplam Site = " + textBox_ToplamLink.Text + "\n");

                    foreach (var item in listBox_BulunanLinkler.Items)
                    {
                        dosyaAc.WriteLine(item);
                    }
                    // Dosya yı kapattık.
                    dosyaAc.Close();
                }
                catch { }
                #endregion

                #region ZafiyetBulunanSiteler
                StreamReader Dosya2 = File.OpenText(hedefYol + @"\ZafiyetBulunanSiteler.txt");
                Dosya2.Close();
                File.Delete(hedefYol + @"\ZafiyetBulunanSiteler.txt");
                try
                {
                    //Dosya2yı appendText ile yazmak için açtık
                    StreamWriter Dosya2Ac = File.AppendText(hedefYol + @"\ZafiyetBulunanSiteler.txt");
                    // Dosya2.WriteLine ile Dosya2ya verileri ekledik.
                    Dosya2Ac.WriteLine("## Zafiyet Bulunan Siteler ##");
                    Dosya2Ac.WriteLine("Kayıt Tarihi= " + DateTime.Now);
                    Dosya2Ac.WriteLine("Toplam Site = " + listBox_AcikBulunanSiteler.Items.Count.ToString() + "\n");

                    foreach (var item in listBox_AcikBulunanSiteler.Items)
                    {
                        Dosya2Ac.WriteLine(item);
                    }
                    // Dosya2 yı kapattık.
                    Dosya2Ac.Close();
                }
                catch { }
                #endregion

                MessageBox.Show("Sonuçlar başarıyla seçdiğiniz dizine kaydedildi.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch
            {
                #region BulunanSiteler.txt
                try
                {
                    //Dosyayı appendText ile yazmak için açtık
                    StreamWriter dosyaAc = File.AppendText(hedefYol + @"\BulunanSiteler.txt");
                    // Dosya.WriteLine ile dosyaya verileri ekledik.
                    dosyaAc.WriteLine("## Dork Taramasında Bulunan Siteler ##");
                    dosyaAc.WriteLine("Kayıt Tarihi= " + DateTime.Now);
                    dosyaAc.WriteLine("Toplam Site = " + textBox_ToplamLink.Text + "\n");

                    foreach (var item in listBox_BulunanLinkler.Items)
                    {
                        dosyaAc.WriteLine(item);
                    }
                    // Dosya yı kapattık.
                    dosyaAc.Close();
                }
                catch { }
                #endregion

                #region BulunanSiteler.txt
                try
                {
                    //Dosya2yı appendText ile yazmak için açtık
                    StreamWriter Dosya2Ac = File.AppendText(hedefYol + @"\ZafiyetBulunanSiteler.txt");
                    // Dosya2.WriteLine ile Dosya2ya verileri ekledik.
                    Dosya2Ac.WriteLine("## Zafiyet Bulunan Siteler ##");
                    Dosya2Ac.WriteLine("Kayıt Tarihi= " + DateTime.Now);
                    Dosya2Ac.WriteLine("Toplam Site = " + listBox_AcikBulunanSiteler.Items.Count.ToString() + "\n");

                    foreach (var item in listBox_AcikBulunanSiteler.Items)
                    {
                        Dosya2Ac.WriteLine(item);
                    }
                    // Dosya2 yı kapattık.
                    Dosya2Ac.Close();
                }
                catch { }
                #endregion

                MessageBox.Show("Sonuçlar başarıyla seçdiğiniz dizine kaydedildi.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Admin Panel
        

        void tarama()
        {
            int kactane = listBox_AdminPanels.Items.Count;

            for (int i = 0; kactane > i; i++)
            {
                if (kactane - 1 == i)
                {
                    button_AdminPanelDurdur.Enabled = false;
                    button_AdminPanelBaslat.Enabled = true;
                    label_Durum.Text = "Admin paneli tarama işlemi tamamlandı.";
                    try
                    {
                        islem.Abort();
                    }
                    catch { }
                }
                else
                {
                    textBox_AdminPanelsTaranan.Text = textBox_AdminPanelSeciliSite.Text + listBox_AdminPanels.Items[i].ToString();

                        HttpWebRequest istek = (HttpWebRequest)HttpWebRequest.Create(textBox_AdminPanelSeciliSite.Text + listBox_AdminPanels.Items[i].ToString());
                        //Proxy
                        if (INI.Read("Ayarlar", "ProxyAktif") == "Evet")
                        {
                            string adres = INI.Read("Ayarlar", "ProxyAdres");
                            string port = INI.Read("Ayarlar", "ProxyPort");
                            istek.Proxy = new WebProxy(adres, int.Parse(port));
                        }

                        //User Agent
                        if (INI.Read("Ayarlar", "UserAgentAktif") == "Evet")
                        {
                            //Rastgele Agent
                            Random rastgele = new Random();
                            listBox_UserAgentList.SelectedIndex = rastgele.Next(0, listBox_UserAgentList.Items.Count);
                            //User Agent
                            istek.UserAgent = listBox_UserAgentList.Text;
                        }

                        //Response Code
                        try
                        {
                            HttpWebResponse cevap = (HttpWebResponse)istek.GetResponse();

                            string durum = cevap.StatusCode.ToString();
                            if (durum == "OK")
                            {
                                int sira = listView1.Items.Count;
                                listView1.Items.Add(listBox_AdminPanels.Items[i].ToString());
                                listView1.Items[sira].SubItems.Add("Başarılı");
                                /*DialogResult soru = MessageBox.Show("Admin Paneli Tespit Edildi!\nSiteyi açmak istiyor musunuz?", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (soru == DialogResult.Yes)
                                {

                                    string key = @"http\shell\open\command";
                                    RegistryKey registryKey =
                                    Registry.ClassesRoot.OpenSubKey(key, false);
                                    string defaultbrowserpath =
                                    ((string)registryKey.GetValue(null, null)).Split('"')[1];
                                    Process.Start(defaultbrowserpath, textBox_AdminPanelSeciliSite.Text + listBox_AdminPanels.Items[i].ToString());
                                }*/
                                label_Durum.Text = "Admin paneli bulundu: " + textBox_AdminPanelSeciliSite.Text + listBox_AdminPanels.Items[i].ToString();
                            }
                        }
                        catch { }
                  
                }
            }
        }
        private void button_AdminPanelBaslat_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            button_AdminPanelDurdur.Enabled = true;
            button_AdminPanelBaslat.Enabled = false;
            textBox_AdminPanelSeciliSite.ReadOnly = true;
            label_Durum.Text = "Admin paneli tarama işlemi gerçekleştiriliyor.";
            islem = new Thread(new ThreadStart(tarama));
            islem.Start();
        }
        private void button_AdminPanelDurdur_Click(object sender, EventArgs e)
        {
            button_AdminPanelDurdur.Enabled = false;
            button_AdminPanelBaslat.Enabled = true;
            textBox_AdminPanelSeciliSite.ReadOnly = false;
            label_Durum.Text = "Admin paneli tarama işlemi durduruldu.";
            islem.Abort();
        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count <= 0)
            {
                return;
            }
            int intselectedindex = listView1.SelectedIndices[0];
            if (intselectedindex >= 0)
            {
                String text = listView1.Items[intselectedindex].Text;
                string key = @"http\shell\open\command";
                RegistryKey registryKey =
                Registry.ClassesRoot.OpenSubKey(key, false);
                string defaultbrowserpath =
                ((string)registryKey.GetValue(null, null)).Split('"')[1];
                Process.Start(defaultbrowserpath, textBox_AdminPanelSeciliSite.Text + listView1.Items[intselectedindex].Text);
            } 
        }
        #endregion

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

                INI INI = new INI(programYol + @"\ayarlar.ini");
                #region Sürüm karşılaştır
                string mevcutSurum = INI.Read("Ayarlar","Surum");
                string bulunanSurum = richTextBox4.Text;
                if (bulunanSurum == mevcutSurum)
                {

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

        #region Taramayı Başlat
        private void button_TaramayiBaslat_Click(object sender, EventArgs e)
        {
            //Dork list veri kontrolü
            if (listBox_DorkList.Items.Count > 0)
            {
                //Hedef Seçim Kontrolü
                if (comboBox_UlkeUzantilari.Text == "Hedef Seçiniz" || comboBox_UlkeUzantilari.Text == "")
                {
                    MessageBox.Show("Hedef seçmeden tarama işlemini başlatamazsınız!", "Hata;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    //Bulunana link kontrolü
                    if (listBox_BulunanLinkler.Items.Count > 0)
                    { 
                        DialogResult soru = MessageBox.Show("Bulunan sitelerde sonuçlar mevcut.\n Zafiyet taraması gerçekleştirmek istiyorsanız 'Evet' butonuna tıklayın,\nİstemiyorsanız 'Hayır' butonuna tıklayarak eski verileri silip yeni tarama başlatabilirsiniz.","Bilgi",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                        if (soru == DialogResult.Yes)
                        {
                            //Zafiyet Taraması Yapılacağında
                            label_Durum.ForeColor = Color.WhiteSmoke;
                            label_Durum.Text = "Zafiyet taraması başlatılıyor..";
                            listBox_BulunanLinkler.Enabled = false;
                            textBox_TarananToplamLink.Text = "0";
                            //Tarama başlat
                            button_TaramayiBaslat.Enabled = false;
                            button_TaramayiDurdur.Enabled = true;
                            //Zafiyet Taraması
                            tZafiyetTaramasi = new Thread(new ThreadStart(vZafiyetTaramasi));
                            tZafiyetTaramasi.Start();
                            //İlk Zaman
                            label_GecenZaman.Text = "Geçen Zaman: [+]";
                        }
                        else if (soru == DialogResult.No)
                        {
                            //Silmek istenirse
                            listBox_BulunanLinkler.Items.Clear();
                            textBox_BulunanLinkSecili.Clear();
                            listBox_AcikBulunanSiteler.Items.Clear();
                            textBox_AcikBulunanSiteSecili.Clear();
                            textBox_ToplamLink.Clear();
                            textBox_TarananToplamLink.Clear();
                            //Tarama başlat
                            button_TaramayiBaslat.Enabled = false;
                            button_TaramayiDurdur.Enabled = true;
                            comboBox_UlkeUzantilari.Enabled = false;
                            //Dork Tarama Başlat
                            timer_DorkTarama.Enabled = true;
                            label_Durum.ForeColor = Color.WhiteSmoke;
                            label_Durum.Text = "Tarama işlemi başlatılıyor..";
                            listBox_DorkList.SelectedIndex = -1;
                            listBox_DorkList.Enabled = false;
                            //İlk Zaman
                            label_GecenZaman.Text = "Geçen Zaman: [+]";
                            ilkZaman = DateTime.Now;
                        }
                    }
                    else
                    {
                        //Tarama başlat
                        button_TaramayiBaslat.Enabled = false;
                        button_TaramayiDurdur.Enabled = true;
                        comboBox_UlkeUzantilari.Enabled = false;
                        //Dork Tarama Başlat
                        timer_DorkTarama.Enabled = true;
                        label_Durum.ForeColor = Color.WhiteSmoke;
                        label_Durum.Text = "Tarama işlemi başlatılıyor..";
                        listBox_DorkList.SelectedIndex = -1;
                        listBox_DorkList.Enabled = false;
                        //İlk Zaman
                        label_GecenZaman.Text = "Geçen Zaman: [+]";
                        ilkZaman = DateTime.Now;
                    }
                }
            }
            else
            {
                MessageBox.Show("Tarama başlatılamadı. Dork listesinde hiç dork bulunamadı!","Bilgi;",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Taramayı Durdur
        private void button_TaramayiDurdur_Click(object sender, EventArgs e)
        { 
            //Tarama durdurulmak istendiğinde
            DialogResult soru = MessageBox.Show("Tarama durdurulsun mu?", "İşlem;", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (soru == DialogResult.Yes)
            {
                //Son Zaman
                sonZaman = DateTime.Now;
                gecen_Zaman();
                //Bulunana link kontrolü
                if (listBox_BulunanLinkler.Items.Count > 0)
                {
                    //Bulunan linkler olduğunda
                    DialogResult soru2 = MessageBox.Show("Eski tarama verileri silinsin mi ?", "İşlem;", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (soru2 == DialogResult.Yes)
                    {
                        //Silmek istenirse
                        listBox_BulunanLinkler.Items.Clear();
                        textBox_BulunanLinkSecili.Clear();
                        listBox_AcikBulunanSiteler.Items.Clear();
                        textBox_AcikBulunanSiteSecili.Clear();
                        textBox_ToplamLink.Clear();
                        textBox_TarananToplamLink.Clear();
                        //Durdurma işlemi
                        button_TaramayiDurdur.Enabled = false;
                        button_TaramayiBaslat.Enabled = true;
                        comboBox_UlkeUzantilari.Enabled = true;
                        //Dork Tarama Durdur
                        timer_DorkTarama.Enabled = false;
                        label_Durum.ForeColor = Color.WhiteSmoke;
                        label_Durum.Text = "Tarama işlemi durduruldu.";
                        listBox_DorkList.SelectedIndex = -1;
                        listBox_DorkList.Enabled = true;
                        try
                        {
                            //Zafiyet Taramasını Kapat
                            tZafiyetTaramasi.Abort();
                            listBox_BulunanLinkler.Enabled = true;
                        }
                        catch
                        {
                            listBox_BulunanLinkler.Enabled = true;
                        }
                    }
                    else if (soru2 == DialogResult.No)
                    {
                        //Silmek istenmezse
                        button_TaramayiDurdur.Enabled = false;
                        button_TaramayiBaslat.Enabled = true;
                        comboBox_UlkeUzantilari.Enabled = true;
                        //Dork Tarama Durdur
                        timer_DorkTarama.Enabled = false;
                        label_Durum.ForeColor = Color.WhiteSmoke;
                        label_Durum.Text = "Tarama işlemi durduruldu.";
                        listBox_DorkList.SelectedIndex = -1;
                        listBox_DorkList.Enabled = true;
                        try
                        {
                            //Zafiyet Taramasını Kapat
                            tZafiyetTaramasi.Abort();
                            listBox_BulunanLinkler.Enabled = true;
                        }
                        catch
                        {
                            listBox_BulunanLinkler.Enabled = true;
                        }
                    }
                }
                else
                {
                    //Hiç bulunan link olmadığında
                    button_TaramayiDurdur.Enabled = false;
                    button_TaramayiBaslat.Enabled = true;
                    comboBox_UlkeUzantilari.Enabled = true;
                    //Dork Tarama Durdur
                    timer_DorkTarama.Enabled = false;
                    label_Durum.ForeColor = Color.WhiteSmoke;
                    label_Durum.Text = "Tarama işlemi durduruldu.";
                    listBox_DorkList.SelectedIndex = -1;
                    listBox_DorkList.Enabled = true;
                    try
                    {
                        //Zafiyet Taramasını Kapat
                        tZafiyetTaramasi.Abort();
                        listBox_BulunanLinkler.Enabled = true;
                    }
                    catch { }
                }
            }
        }
        #endregion

        #region Google Dork Taraması

        #region Timer DorkTarama
        int dorkSure = 0;
        private void timer_DorkTarama_Tick(object sender, EventArgs e)
        {
            try
            {
                //Dork listesinin sonuna geldiğinde
                if (listBox_DorkList.Items.Count - 1 == listBox_DorkList.SelectedIndex)
                {
                    //Dork Taraması bittiğinde
                    dorkSure = 0;
                    timer_DorkTarama.Enabled = false;
                    webBrowser_DorkTarama.Stop();
                    label_Durum.ForeColor = Color.WhiteSmoke;
                    label_Durum.Text = "Dork tarama işlemi tamamlandı. [" + listBox_BulunanLinkler.Items.Count.ToString() + "] site bulundu.";

                    DialogResult soru = MessageBox.Show("Dork tarama işlemi tamamlandı. [" + listBox_BulunanLinkler.Items.Count.ToString() + "] site bulundu. \nBulunan siteler SQL Injection taraması yapmak istiyor musunuz?", "İşlem;", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (soru == DialogResult.Yes)
                    {
                        //Zafiyet Taraması Yapılacağında
                        label_Durum.ForeColor = Color.WhiteSmoke;
                        label_Durum.Text = "Zafiyet taraması başlatılıyor..";
                        listBox_BulunanLinkler.Enabled = false;
                        textBox_TarananToplamLink.Text = "0";
                        //Zafiyet Taraması
                        tZafiyetTaramasi = new Thread(new ThreadStart(vZafiyetTaramasi));
                        tZafiyetTaramasi.Start();
                    }
                    else
                    {
                        //Zafiyet Taraması Yapmak İstemediğinde
                        label_Durum.ForeColor = Color.DarkRed;
                        label_Durum.Text = "Zafiyet tarama işlemi iptal edildi.";
                        button_TaramayiDurdur.Enabled = false;
                        button_TaramayiBaslat.Enabled = true;
                        comboBox_UlkeUzantilari.Enabled = true;
                        listBox_DorkList.SelectedIndex = -1;
                        listBox_DorkList.Enabled = true;
                        //Son Zaman
                        sonZaman = DateTime.Now;
                        gecen_Zaman();
                    }
                }
                else
                {
                    //dorkSure 1 saniye arttır
                    dorkSure = dorkSure + 1;
                    label_zaman.Text = dorkSure.ToString();

                    //Bekleme süresiyle eşitlendiğinde
                    if (dorkSure == int.Parse(textBox_MaxSure.Text))
                    {
                        //Dorku bir arttır
                        listBox_DorkList.SelectedIndex = listBox_DorkList.SelectedIndex + 1;
                        //Dork düzenle
                        string dork = listBox_DorkList.Text;
                        dork = dork.Replace("?", "%3F");
                        dork = dork.Replace("=", "%3D");
                        dork = dork.Replace(";", "%3B");
                        dork = dork.Replace("/", "%2F");
                        dork = dork.Replace(textBox_DorkTaramaOzel.Text, "%22");
                        dork = dork.Replace(" ", "+");

                        //Ülke uzantısı konrolü
                        if (comboBox_UlkeUzantilari.Text == ". Global Arama")
                        {
                            webBrowser_DorkTarama.Navigate("https://www.google.com.tr/search?q=inurl:" + dork + "&num=" + textBox_MaxSite.Text + "&gws_rd=ssl");
                        }
                        else
                        {
                            //Ülke uzantısı
                            hedefUlkeUzantisi = comboBox_UlkeUzantilari.Text.Substring(0, 3);
                            webBrowser_DorkTarama.Navigate("https://www.google.com.tr/search?q=inurl:" + dork + "site:" + hedefUlkeUzantisi + "&num=" + textBox_MaxSite.Text + "&gws_rd=ssl");
                        }
                    }

                }

                if (dorkSure == 30)
                {
                    dorkSure = 0;
                }
            }
            catch { }
        }
        #endregion

        #region webBrowser Dork Tarama
        private void webBrowser_DorkTarama_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            try
            {
                //Dork taranmaya başlandığında
                label_Durum.ForeColor = Color.WhiteSmoke;
                label_Durum.Text = listBox_DorkList.Text + " dorku taranıyor..";
                
                //Site kodlarını aktar
                string html = webBrowser_DorkTarama.Document.Body.InnerHtml.ToString();
                richTextBox_DorkTaramaHTML.Text = html;
                //Site kodları içinde ara
                HtmlAgilityPack.HtmlDocument dokuman = new HtmlAgilityPack.HtmlDocument();
                dokuman.LoadHtml(html);
                HtmlNodeCollection XPath = dokuman.DocumentNode.SelectNodes("//h3[@class='r']");
                foreach (var veri in XPath)
                {
                   //Bulduklarını listele
                    string link = veri.InnerHtml;
                    //Sadeleştir
                    string gelen = link;
                    int titleIndexBaslangici = gelen.IndexOf(textBox_DorkTaramaIlkDeger.Text) + textBox_DorkTaramaIlkDeger.TextLength;
                    int titleIndexBitisi = gelen.Substring(titleIndexBaslangici).IndexOf(textBox_DorkTaramaIkinciDeger.Text);
                    link = gelen.Substring(titleIndexBaslangici, titleIndexBitisi);
                    if (link.IndexOf("&amp") != -1)
                    {
                        string gelen3 = link;
                        int titleIndexBaslangici3 = gelen3.IndexOf("http://") + 7;
                        int titleIndexBitisi3 = gelen3.Substring(titleIndexBaslangici3).IndexOf("&amp");
                        link = gelen3.Substring(titleIndexBaslangici3, titleIndexBitisi3);

                        link = link.Replace("//", "");
                    }
                    
                    //Daha önce eklenmemişse ekle
                    if (listBox_BulunanLinkler.Items.Contains(link) == false)
                    {
                        link = link.Replace("http://", "");
                        listBox_BulunanLinkler.Items.Add("http://" + link);
                    }
                }
                //Dork tarama işlemi tamamlandığında
                dorkSure = 0;
                //Bulunan linklerin toplamı
                textBox_ToplamLink.Text = listBox_BulunanLinkler.Items.Count.ToString();
                label_Durum.ForeColor = Color.WhiteSmoke;
                label_Durum.Text = listBox_DorkList.Text + " dorku tamamlandı. Toplam site: " + XPath.Count();
            }
            catch 
            {
                label_Durum.ForeColor = Color.DarkRed;
                label_Durum.Text = listBox_DorkList.Text + " dorkuna ait hiç site bulunamadı.";
                dorkSure = 0;
            }
        }
        #endregion

        #endregion

        #region Zafiyet Taraması
        void vZafiyetTaramasi()
        {
            try
            {
                int kactane = listBox_BulunanLinkler.Items.Count;
                for (int i = 0; kactane > i; i++)
                {
                    string url = listBox_BulunanLinkler.Items[i].ToString();
                    label_Durum.ForeColor = Color.WhiteSmoke;
                    label_Durum.Text = url + " sitesi taranıyor...";
                    int sayi = int.Parse(textBox_TarananToplamLink.Text) + 1;
                    textBox_TarananToplamLink.Text = sayi.ToString();

                    if (url.IndexOf(".php?") != -1)
                    {
                        HttpWebRequest istek = (HttpWebRequest)HttpWebRequest.Create(url + textBox_MySQLPayload.Text);
                        #region İstek
                        //Proxy
                        if (INI.Read("Ayarlar", "ProxyAktif") == "Evet")
                        {
                            string adres = INI.Read("Ayarlar", "ProxyAdres");
                            string port = INI.Read("Ayarlar", "ProxyPort");
                            istek.Proxy = new WebProxy(adres, int.Parse(port));
                        }

                        //User Agent
                        if (INI.Read("Ayarlar", "UserAgentAktif") == "Evet")
                        {
                            //Rastgele Agent
                            Random rastgele = new Random();
                            listBox_UserAgentList.SelectedIndex = rastgele.Next(0, listBox_UserAgentList.Items.Count);
                            //User Agent
                            istek.UserAgent = listBox_UserAgentList.Text;
                        }

                        try
                        {
                            //Response Code
                            HttpWebResponse cevap = (HttpWebResponse)istek.GetResponse();

                            if (cevap.StatusCode == HttpStatusCode.OK)
                            {
                                Stream receiveStream = cevap.GetResponseStream();
                                StreamReader readStream = null;

                                if (cevap.CharacterSet == null)
                                {
                                    readStream = new StreamReader(receiveStream);
                                }
                                else
                                {
                                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(cevap.CharacterSet));
                                }

                                string data = readStream.ReadToEnd();

                                cevap.Close();
                                readStream.Close();

                                string html = data;

                                string[] arananlar = 
                    { 
                        "mysql", "mssql", "syntax", "warning", "SQL", 
                        "MySQL", "MSSQL", "Warning:", "mysql_fetch", 
                        "resorce", "boolean", "JET", "Engine error", 
                        "OLE DB Provider" 
                    };

                                for (int ii = 0; arananlar.Count() > ii; ii++)
                                {
                                    if (data.IndexOf(arananlar[ii]) != -1)
                                    {
                                        if (listBox_AcikBulunanSiteler.Items.Contains(listBox_BulunanLinkler.Items[i].ToString()) == false)
                                        {
                                            listBox_AcikBulunanSiteler.Items.Add(listBox_BulunanLinkler.Items[i].ToString());
                                        }
                                        label_Durum.ForeColor = Color.DarkGreen;
                                        label_Durum.Text = listBox_BulunanLinkler.Items[i].ToString() + " sitesinde SQL Injection zafiyeti tespit edildi.";
                                    }
                                }
                            }
                        }
                        catch { }
                        #endregion
                    }
                    else if (url.IndexOf(".asp?") != -1)
                    {
                        HttpWebRequest istek = (HttpWebRequest)HttpWebRequest.Create(url + textBox_MSSQLPayload.Text);
                        #region İstek
                        //Proxy
                        if (INI.Read("Ayarlar", "ProxyAktif") == "Evet")
                        {
                            string adres = INI.Read("Ayarlar", "ProxyAdres");
                            string port = INI.Read("Ayarlar", "ProxyPort");
                            istek.Proxy = new WebProxy(adres, int.Parse(port));
                        }

                        //User Agent
                        if (INI.Read("Ayarlar", "UserAgentAktif") == "Evet")
                        {
                            //Rastgele Agent
                            Random rastgele = new Random();
                            listBox_UserAgentList.SelectedIndex = rastgele.Next(0, listBox_UserAgentList.Items.Count);
                            //User Agent
                            istek.UserAgent = listBox_UserAgentList.Text;
                        }

                        try
                        {
                            //Response Code
                            HttpWebResponse cevap = (HttpWebResponse)istek.GetResponse();

                            if (cevap.StatusCode == HttpStatusCode.OK)
                            {
                                Stream receiveStream = cevap.GetResponseStream();
                                StreamReader readStream = null;

                                if (cevap.CharacterSet == null)
                                {
                                    readStream = new StreamReader(receiveStream);
                                }
                                else
                                {
                                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(cevap.CharacterSet));
                                }

                                string data = readStream.ReadToEnd();

                                cevap.Close();
                                readStream.Close();

                                string html = data;

                                string[] arananlar = 
                    { 
                        "mysql", "mssql", "syntax", "warning", "SQL", 
                        "MySQL", "MSSQL", "Warning:", "mysql_fetch", 
                        "resorce", "boolean", "JET", "Engine error", 
                        "OLE DB Provider" 
                    };

                                for (int ii = 0; arananlar.Count() > ii; ii++)
                                {
                                    if (data.IndexOf(arananlar[ii]) != -1)
                                    {
                                        if (listBox_AcikBulunanSiteler.Items.Contains(listBox_BulunanLinkler.Items[i].ToString()) == false)
                                        {
                                            listBox_AcikBulunanSiteler.Items.Add(listBox_BulunanLinkler.Items[i].ToString());
                                        }
                                        label_Durum.ForeColor = Color.DarkGreen;
                                        label_Durum.Text = listBox_BulunanLinkler.Items[i].ToString() + " sitesinde SQL Injection zafiyeti tespit edildi.";
                                    }
                                }
                            }
                        }
                        catch { }
                        #endregion
                    }
                    else if (url.IndexOf(".aspx?") != -1)
                    {
                        HttpWebRequest istek = (HttpWebRequest)HttpWebRequest.Create(url + textBox_MSSQLPayload.Text);
                        #region İstek
                        //Proxy
                        if (INI.Read("Ayarlar", "ProxyAktif") == "Evet")
                        {
                            string adres = INI.Read("Ayarlar", "ProxyAdres");
                            string port = INI.Read("Ayarlar", "ProxyPort");
                            istek.Proxy = new WebProxy(adres, int.Parse(port));
                        }

                        //User Agent
                        if (INI.Read("Ayarlar", "UserAgentAktif") == "Evet")
                        {
                            //Rastgele Agent
                            Random rastgele = new Random();
                            listBox_UserAgentList.SelectedIndex = rastgele.Next(0, listBox_UserAgentList.Items.Count);
                            //User Agent
                            istek.UserAgent = listBox_UserAgentList.Text;
                        }

                        try
                        {
                            //Response Code
                            HttpWebResponse cevap = (HttpWebResponse)istek.GetResponse();

                            if (cevap.StatusCode == HttpStatusCode.OK)
                            {
                                Stream receiveStream = cevap.GetResponseStream();
                                StreamReader readStream = null;

                                if (cevap.CharacterSet == null)
                                {
                                    readStream = new StreamReader(receiveStream);
                                }
                                else
                                {
                                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(cevap.CharacterSet));
                                }

                                string data = readStream.ReadToEnd();

                                cevap.Close();
                                readStream.Close();

                                string html = data;

                                string[] arananlar = 
                    { 
                        "mysql", "mssql", "syntax", "warning", "SQL", 
                        "MySQL", "MSSQL", "Warning:", "mysql_fetch", 
                        "resorce", "boolean", "JET", "Engine error", 
                        "OLE DB Provider" 
                    };

                                for (int ii = 0; arananlar.Count() > ii; ii++)
                                {
                                    if (data.IndexOf(arananlar[ii]) != -1)
                                    {
                                        if (listBox_AcikBulunanSiteler.Items.Contains(listBox_BulunanLinkler.Items[i].ToString()) == false)
                                        {
                                            listBox_AcikBulunanSiteler.Items.Add(listBox_BulunanLinkler.Items[i].ToString());
                                        }
                                        label_Durum.ForeColor = Color.DarkGreen;
                                        label_Durum.Text = listBox_BulunanLinkler.Items[i].ToString() + " sitesinde SQL Injection zafiyeti tespit edildi.";
                                    }
                                }
                            }
                        }
                        catch { }
                        #endregion
                    }
                    else if (url.IndexOf("/?") != -1)
                    {
                        HttpWebRequest istek = (HttpWebRequest)HttpWebRequest.Create(url + textBox_MySQLPayload.Text);
                        #region İstek
                        //Proxy
                        if (INI.Read("Ayarlar", "ProxyAktif") == "Evet")
                        {
                            string adres = INI.Read("Ayarlar", "ProxyAdres");
                            string port = INI.Read("Ayarlar", "ProxyPort");
                            istek.Proxy = new WebProxy(adres, int.Parse(port));
                        }

                        //User Agent
                        if (INI.Read("Ayarlar", "UserAgentAktif") == "Evet")
                        {
                            //Rastgele Agent
                            Random rastgele = new Random();
                            listBox_UserAgentList.SelectedIndex = rastgele.Next(0, listBox_UserAgentList.Items.Count);
                            //User Agent
                            istek.UserAgent = listBox_UserAgentList.Text;
                        }

                        try
                        {
                            //Response Code
                            HttpWebResponse cevap = (HttpWebResponse)istek.GetResponse();

                            if (cevap.StatusCode == HttpStatusCode.OK)
                            {
                                Stream receiveStream = cevap.GetResponseStream();
                                StreamReader readStream = null;

                                if (cevap.CharacterSet == null)
                                {
                                    readStream = new StreamReader(receiveStream);
                                }
                                else
                                {
                                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(cevap.CharacterSet));
                                }

                                string data = readStream.ReadToEnd();

                                cevap.Close();
                                readStream.Close();

                                string html = data;

                                string[] arananlar = 
                    { 
                        "mysql", "mssql", "syntax", "warning", "SQL", 
                        "MySQL", "MSSQL", "Warning:", "mysql_fetch", 
                        "resorce", "boolean", "JET", "Engine error", 
                        "OLE DB Provider" 
                    };

                                for (int ii = 0; arananlar.Count() > ii; ii++)
                                {
                                    if (data.IndexOf(arananlar[ii]) != -1)
                                    {
                                        if (listBox_AcikBulunanSiteler.Items.Contains(listBox_BulunanLinkler.Items[i].ToString()) == false)
                                        {
                                            listBox_AcikBulunanSiteler.Items.Add(listBox_BulunanLinkler.Items[i].ToString());
                                        }
                                        label_Durum.ForeColor = Color.DarkGreen;
                                        label_Durum.Text = listBox_BulunanLinkler.Items[i].ToString() + " sitesinde SQL Injection zafiyeti tespit edildi.";
                                    }
                                }
                            }
                        }
                        catch { }
                        #endregion
                    }
                    else
                    {
                        label_Durum.ForeColor = Color.DarkRed;
                        label_Durum.Text = listBox_BulunanLinkler.Text + " bu site atlandı.";
                    }
                }
                //Son Zaman
                sonZaman = DateTime.Now;
                gecen_Zaman();
                //Tamamlandığında
                label_Durum.Text = "Zafiyet tarama işlemi tamamlandı. [" + listBox_AcikBulunanSiteler.Items.Count.ToString() + "] site bulundu.";
                DialogResult soru = MessageBox.Show("Zafiyet tarama işlemi tamamlandı. [" + listBox_AcikBulunanSiteler.Items.Count.ToString() + "] site bulundu.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (soru == DialogResult.OK)
                {
                    //Silmek istenmezse
                    button_TaramayiDurdur.Enabled = false;
                    button_TaramayiBaslat.Enabled = true;
                    comboBox_UlkeUzantilari.Enabled = true;
                    //Zafiyet Tarama bitti
                    timer_DorkTarama.Enabled = false;
                    label_Durum.ForeColor = Color.WhiteSmoke;
                    label_Durum.Text = "Zafiyet taraması tamamlandı.";
                    listBox_DorkList.SelectedIndex = -1;
                    listBox_DorkList.Enabled = true;
                    listBox_BulunanLinkler.Enabled = true;
                    //Zafiyet Taramasını Kapat
                    tZafiyetTaramasi.Abort();
                }
            }
            catch { }
        }
        #endregion

        #region Eğlence
        int eglence = 0;
        private void timer3_Tick(object sender, EventArgs e)
        {
            eglence = eglence + 1;
            if (label_GecenZaman.Text.IndexOf("[") != -1)
            {
                if (eglence == 1)
                {
                    label_GecenZaman.Text = "Geçen Zaman: [/]";
                }
                if (eglence == 2)
                {
                    label_GecenZaman.Text = @"Geçen Zaman: [\]";
                }
                if (eglence == 3)
                {
                    label_GecenZaman.Text = "Geçen Zaman: [.]";
                }
                if (eglence == 4)
                {
                    label_GecenZaman.Text = "Geçen Zaman: [./]";
                }
                if (eglence == 5)
                {
                    label_GecenZaman.Text = @"Geçen Zaman: [.\]";
                }
                if (eglence == 6)
                {
                    label_GecenZaman.Text = "Geçen Zaman: [..]";
                }
                if (eglence == 7)
                {
                    label_GecenZaman.Text = "Geçen Zaman: [../]";
                }
                if (eglence == 8)
                {
                    label_GecenZaman.Text = @"Geçen Zaman: [..\]";
                }
                if (eglence == 9)
                {
                    label_GecenZaman.Text = "Geçen Zaman: [...]";
                }
                if (eglence == 10)
                {
                    label_GecenZaman.Text = "Geçen Zaman: [.../]";
                }
                if (eglence == 11)
                {
                    label_GecenZaman.Text = @"Geçen Zaman: [...\]";
                }
                if (eglence == 12)
                {
                    label_GecenZaman.Text = "Geçen Zaman: [....]";
                }
            }

            if (eglence == 13)
            {
                eglence = 0;
            }
        }
        #endregion



    }   
}
