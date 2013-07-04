using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using THOK.MCP.Config;
using THOK.XC.Process;
using THOK.XC.Process.Util;
using THOK.Util;

namespace THOK.XC.Dispatching.View
{
    public partial class ParameterForm : THOK.AF.View.ToolbarForm
    {
        private THOK.XC.Process.Parameter parameter = new THOK.XC.Process.Parameter();
        private DBConfigUtil config = new DBConfigUtil("DefaultConnection", "SQLSERVER");
        private DBConfigUtil serverConfig = new DBConfigUtil("ServerConnection", "SQLSERVER");

        private Dictionary<string, string> attributes = null;

        public ParameterForm()
        {
            InitializeComponent();
            ReadParameter();
        }

        private void ReadParameter()
        {
            //��ȡContext�����ļ�LED��ʾ������
            ConfigUtil configUtil = new ConfigUtil();
            attributes = configUtil.GetAttribute();
            parameter.LED_01_CHANNELCODE = attributes["LED_01_CHANNELCODE"];
            parameter.LED_02_CHANNELCODE = attributes["LED_02_CHANNELCODE"];
            parameter.SupplyToSortLine = attributes["SupplyToSortLine"];

            //�������ݿ����Ӳ���
            parameter.ServerName = config.Parameters["server"].ToString();
            parameter.DBName = config.Parameters["database"].ToString();
            parameter.DBUser = config.Parameters["uid"].ToString();
            parameter.Password = config.Parameters["password"].ToString();

            //���������ݿ����Ӳ���
            parameter.RemoteServerName = serverConfig.Parameters["server"].ToString();
            parameter.RemoteDBName = serverConfig.Parameters["database"].ToString();
            parameter.RemoteDBUser = serverConfig.Parameters["uid"].ToString();
            parameter.RemotePassword = serverConfig.Parameters["password"].ToString();


            propertyGrid.SelectedObject = parameter;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //����Context�����ļ�LED��ʾ������
                attributes["LED_01_CHANNELCODE"] = parameter.LED_01_CHANNELCODE;
                attributes["LED_02_CHANNELCODE"] = parameter.LED_02_CHANNELCODE;
                attributes["SupplyToSortLine"] = parameter.SupplyToSortLine;

                ConfigUtil configUtil = new ConfigUtil();
                configUtil.Save(attributes);

                //���汾�����ݿ����Ӳ���
                config.Parameters["server"] = parameter.ServerName;
                config.Parameters["database"] = parameter.DBName;
                config.Parameters["uid"] = parameter.DBUser;
                config.Parameters["Password"] = config.Parameters["Password"].ToString() == parameter.Password?parameter.Password: THOK.Util.Coding.Encoding(parameter.Password);
                config.Save();

                //������������ݿ����Ӳ���
                serverConfig.Parameters["server"] = parameter.RemoteServerName;
                serverConfig.Parameters["database"] = parameter.RemoteDBName;
                serverConfig.Parameters["uid"] = parameter.RemoteDBUser;
                serverConfig.Parameters["Password"] = serverConfig.Parameters["Password"].ToString() == parameter.RemotePassword ? parameter.RemotePassword:THOK.Util.Coding.Encoding(parameter.RemotePassword);
                serverConfig.Save();   


                MessageBox.Show("ϵͳ��������ɹ���������������ϵͳ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exp)
            {
                MessageBox.Show("����ϵͳ���������г����쳣��ԭ��" + exp.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Exit();
        }
    }
}

