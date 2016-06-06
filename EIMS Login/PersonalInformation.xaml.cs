﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Data.SqlClient;

namespace EIMS_Login
{
    /// <summary>
    /// PersonalInformation.xaml 的交互逻辑
    /// </summary>
    public partial class PersonalInformation : UserControl
    {
        private byte[] _imageBinary;
        private string _imgLocalPath;

        public PersonalInformation()
        {
            InitializeComponent();
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "*jpg|*.JPG|*.GIF|*.GIF|*.BMP|*.BMP|*.png|*.PNG";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                FileStream fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                this._imageBinary = br.ReadBytes((int)fs.Length);
                this._imgLocalPath = dialog.FileName;
                br.Close();
                fs.Close();
                this.MyImage.Source = ByteArrayToBitmapImage(this._imageBinary);
            }
        }
        BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            BitmapImage bmp = null;

            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }
            return bmp;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Connection Temp = new Connection();
            EIMS_Login.Ordinaryusers.OrdinaryUserInfo NowUser = new Ordinaryusers.OrdinaryUserInfo();
            try
            {
                string UserStrSql = "select * from ArmsUser where Ryid='" + NowUser.UserInfoTemp.Ryid + "'";
                SqlCommand CMD_1 = new SqlCommand(UserStrSql, Temp.GetConn());
                SqlDataReader Sdr_1 = CMD_1.ExecuteReader();
                if (Sdr_1.Read())
                {
                    this.Account.Text = Sdr_1[0].ToString();
                    Sdr_1.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("拉取个人信息失败！" + ex);
            }
            this.Name.Text = NowUser.UserInfoTemp.RyName;
            this.Serialnumber.Text = NowUser.UserInfoTemp.Ryid;
            if(NowUser.UserInfoTemp.Sex == 0)
            {
                this.Gender.Text = "男";
            }
            else if(NowUser.UserInfoTemp.Sex == 1)
            {
                this.Gender.Text = "女";
            }
            else
            {
                this.Gender.Text = "未知";
            }
            this.Departmentnumber.Text = NowUser.UserInfoTemp.Dep_Id.ToString();
            this.IDcard.Text = NowUser.UserInfoTemp.Id_Card;
            this.National.Text = NowUser.UserInfoTemp.Nationalty;
            this.Birthday.Text = NowUser.UserInfoTemp.Birth;
            this.Position.Text = NowUser.UserInfoTemp.Title;
            this.Rank.Text = NowUser.UserInfoTemp.Rank;
            this.Politicallandscape.Text = NowUser.UserInfoTemp.Political_Party;
            this.Leveleducation.Text = NowUser.UserInfoTemp.Culture_Level;
            this.Maritalstatus.Text = NowUser.UserInfoTemp.Marital_Condition.ToString();
            this.Nativeplace.Text = NowUser.UserInfoTemp.Family_Place;
            this.Post.Text = NowUser.UserInfoTemp.Position;
            this.LeaderNumber.Text = NowUser.UserInfoTemp.UpperId;
            if(NowUser.UserInfoTemp.Photo == null)
            {
                MessageBox.Show("没有图片");
            }
            else
            {
                this.MyImage.Source = NowUser.UserInfoTemp.Photo;
            }
        }
    }
}