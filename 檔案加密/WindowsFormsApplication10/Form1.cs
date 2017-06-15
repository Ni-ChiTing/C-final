using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using System.IO;
namespace WindowsFormsApplication10
{
    public partial class Form1 : Form
    {
         private string  password="";
        public Form1()
        {
            InitializeComponent();

        }
        string []filepath;
        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                filepath = openFileDialog1.FileNames;
                string t = "";
                foreach(string s in filepath)
                {
                    t += s;
                }
                MessageBox.Show(t, "開起檔案路徑");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            password = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (filepath == null)
            {
                MessageBox.Show("請選取檔案", "warning", MessageBoxButtons.OK);
            }
            else
            { 
            if (password.Equals(""))
            {
                MessageBox.Show("密碼不得為空", "warning", MessageBoxButtons.OK);
            }
           else
            {
                byte[] pwd = Encoding.Unicode.GetBytes(password);
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
                byte[] key = sha256.ComputeHash(pwd);//32
                byte[] iv = md5.ComputeHash(pwd);//16
                provider.Key = key;
                provider.IV = iv;
                BinaryReader br;
                 foreach (string s in filepath)
                 {
                        if(File.Exists(s))
                        {
                            FileStream fs = new FileStream(s, FileMode.Open);
                            FileInfo fi = new FileInfo(s);
                            br = new BinaryReader(fs);
                            string path = fi.FullName + ".encrypt";
                            MessageBox.Show(path, "path");
                            FileStream fss = new FileStream(path, FileMode.Create);
                            BinaryWriter bw = new BinaryWriter(fss);
                            while(br.BaseStream.Position < br.BaseStream.Length)
                            {
                                int dl = System.Convert.ToInt32(fs.Length);
                                byte[] ss;
                                byte[] encrytext;
                                ss = br.ReadBytes(dl);
                                encrytext = AesEncryptor(ss, key, iv);
                                bw.Write(encrytext);
                                bw.Flush();
                               
                            }

                            
                            br.Close();
                            fs.Close();
                            bw.Flush();
                            bw.Close();
                            fss.Close();
                            System.IO.File.Delete(s);
                            textBox1.Text = "";
                            textBox2.Text = "";
                        }
                         
                 }
                }
            }
        }

        public byte[] AesEncryptor(byte[] bsFile,byte[]key,byte[] iv)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform transform = aes.CreateEncryptor();
            return transform.TransformFinalBlock(bsFile, 0, bsFile.Length);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filepath = openFileDialog1.FileNames;
                string t = "";
                foreach (string s in filepath)
                {
                    t += s;
                }
                MessageBox.Show(t, "開起檔案路徑");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            password = textBox2.Text;
        }
        public byte[] AesDecryptor(byte[] bsFile, byte[] key, byte[] iv)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform transform = aes.CreateDecryptor();
            return transform.TransformFinalBlock(bsFile, 0, bsFile.Length);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (filepath == null)
            {
                MessageBox.Show("請選取檔案", "warning", MessageBoxButtons.OK);
            }
            else
            {
                if (password.Equals(""))
                {
                    MessageBox.Show("密碼不得為空", "warning", MessageBoxButtons.OK);
                }
                else
                {
                    byte[] pwd = Encoding.Unicode.GetBytes(password);
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                    AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
                    byte[] key = sha256.ComputeHash(pwd);//32
                    byte[] iv = md5.ComputeHash(pwd);//16
                    provider.Key = key;
                    provider.IV = iv;
                    BinaryReader br;
                    foreach (string s in filepath)
                    {
                        int cp = s.IndexOf(".encrypt");
                        if(cp>=0)
                        {
                            if (File.Exists(s))
                            {
                                FileStream fs = new FileStream(s, FileMode.Open);
                                FileInfo fi = new FileInfo(s);
                                br = new BinaryReader(fs);
                                string path = fi.FullName;
                                char sp = '.';
                                string[] temp;
                                temp = path.Split(sp);
                                path = temp[0] + "." + temp[1];
                                MessageBox.Show(path, "path");
                                FileStream fss = new FileStream(path, FileMode.Create);
                                BinaryWriter bw = new BinaryWriter(fss);
                                try
                                {
                                    while (br.BaseStream.Position < br.BaseStream.Length)
                                    {
                                        int dl = System.Convert.ToInt32(fs.Length);
                                        byte[] ss;
                                        byte[] plaintext;
                                        ss = br.ReadBytes(dl);
                                        plaintext = AesDecryptor(ss, key, iv);
                                        bw.Write(plaintext);
                                        bw.Flush();

                                    }
                                }
                                catch(Exception ee)
                                {
                                    MessageBox.Show(ee.Message);
                                }

                                br.Close();
                                fs.Close();
                                System.IO.File.Delete(s);
                                 bw.Flush();
                                bw.Close();
                                 fss.Close();
                                textBox1.Text = "";
                                textBox2.Text = "";
                            }
                    
                        }
                        else
                        {
                            MessageBox.Show("此為非加密檔案", "warning", MessageBoxButtons.OK);
                        }

                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            textBox1.Text = "";
            label1.BackColor = Color.Transparent;
        }
    }
}
