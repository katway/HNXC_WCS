using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using THOK.MCP;
using THOK.MCP.View;
using THOK.Util;
using THOK.XC.Process.Dal;

namespace THOK.XC.Dispatching.View
{
    public partial class ButtonArea : ProcessControl
    {
        private int IndexStar = 0;
        public ButtonArea()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (btnStop.Enabled)
            {
                MessageBox.Show("��ֹͣ��������˳�ϵͳ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (DialogResult.Yes == MessageBox.Show("��ȷ��Ҫ�˳��������ϵͳ��", "ѯ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                THOK.XC.Dispatching.Util.LogFile.DeleteFile();
                Application.Exit();
            }
        }

        private void btnOperate_Click(object sender, EventArgs e)
        {
            try
            {
                THOK.AF.Config config = new THOK.AF.Config();
                THOK.AF.MainFrame mainFrame = new THOK.AF.MainFrame(config);
                mainFrame.Context = Context;
                mainFrame.ShowInTaskbar = false;
                mainFrame.Icon = new Icon(@"./App.ico");
                mainFrame.ShowIcon = true;
                mainFrame.StartPosition = FormStartPosition.CenterScreen;
                mainFrame.WindowState = FormWindowState.Maximized;
                mainFrame.ShowDialog();
            }
            catch (Exception ee)
            {
                Logger.Error("������ҵ����ʧ�ܣ�ԭ��" + ee.Message);
            }
        }

       

       
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                Context.ProcessDispatcher.WriteToProcess("OrderDataStateProcess", "Start", null);
                Context.ProcessDispatcher.WriteToProcess("LEDProcess", "Refresh", null);
                Context.ProcessDispatcher.WriteToProcess("LedStateProcess", "Refresh", null);
                SwitchStatus(true);
                timer1.Enabled = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Context.ProcessDispatcher.WriteToProcess("OrderDataStateProcess", "Stop", null);
            SwitchStatus(false);
            timer1.Enabled = false;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "help.chm");
        }

        private void SwitchStatus(bool isStart)
        {
            btnCheckScan.Enabled = !isStart;
            btnPalletIn.Enabled = !isStart;
            btnStart.Enabled = !isStart;
            btnStop.Enabled = isStart;
            btnSimulate.Enabled = !isStart;
        }

        private void btnSimulate_Click(object sender, EventArgs e)
        {            
            try
            {
               
            }
            catch (Exception ee)
            {
                Logger.Error("���PLCδɨ�������Ϣ����ʧ�ܣ�ԭ��" + ee.Message);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            TaskDal taskDal = new TaskDal();
            DataTable dt = taskDal.TaskOutToDetail();
            DataTable dt2 = null;
            if (IndexStar == 0)
            {
                string strWhere = string.Format("TASK_TYPE IN ({0}) AND DETAIL.STATE IN ({1})", "11,21,12", "0,1");
                dt2 = taskDal.TaskCraneDetail(strWhere);
                strWhere = string.Format("TASK_TYPE IN ({0}) AND DETAIL.STATE IN ({1})", "22", "0,1,2");
                DataTable dtout = taskDal.TaskCraneDetail(strWhere);
                dt2.Merge(dtout);
            }
            DataTable[] dtSend = new DataTable[2];
            dtSend[0] = dt;
            dtSend[1] = dt2;
            Context.ProcessDispatcher.WriteToProcess("CraneProcess", "StockOutRequest", dtSend);
            IndexStar++;
        }
        
        /// <summary>
        /// ������ⷽʽ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPalletIn_Click(object sender, EventArgs e)
        {
             PalletSelect frm = new PalletSelect();
             if (frm.ShowDialog() == DialogResult.OK)
             {
                 if (frm.Flag == 1) //���������
                 {
                     string writeItem = "01_2_122_";
                     int[] ServiceW = new int[3];
                     ServiceW[0] =9999; //�����
                     ServiceW[1] = 131;//Ŀ�ĵ�ַ
                     ServiceW[2] = 4;


                     Context.ProcessDispatcher.WriteToService("StockPLC_01", writeItem + "1", ServiceW); //PLCд������
                     Context.ProcessDispatcher.WriteToService("StockPLC_01", writeItem + "2", 1); //PLCд������
                 }
                 else if (frm.Flag == 2)
                 {
                     PalletBillDal Billdal = new PalletBillDal();
                     string TaskID = Billdal.CreatePalletInBillTask(true); //����������ⵥ������Task.
                     string FromStation = "122";
                     string ToStation = "122";
                     string writeItem = "01_2_122_";


                     string strWhere = string.Format("TASK_ID='{0}'", TaskID);
                     TaskDal dal = new TaskDal();
                     string[] strValue = dal.AssignCell(strWhere,"122");//��λ����

                     dal.UpdateTaskState(strValue[0], "1");//��������ʼִ��
                     ProductStateDal StateDal = new ProductStateDal();
                     StateDal.UpdateProductCellCode(strValue[0], strValue[4]); //����Product_State ��λ
                     dal.UpdateTaskDetailStation(FromStation, ToStation, "2", string.Format("TASK_ID='{0}' AND ITEM_NO=1", strValue[0])); //���»�λ������ʼ��ַ��Ŀ���ַ��

                     int[] ServiceW = new int[3];
                     ServiceW[0] = int.Parse(strValue[1]); //�����
                     ServiceW[1] = int.Parse(strValue[2]);//Ŀ�ĵ�ַ
                     ServiceW[2] = 2;
                    

                     Context.ProcessDispatcher.WriteToService("StockPLC_01", writeItem + "1", ServiceW); //PLCд������
                     Context.ProcessDispatcher.WriteToService("StockPLC_01", writeItem + "2", 1); //PLCд������

                     dal.UpdateTaskDetailStation(ToStation, strValue[2], "1", string.Format("TASK_ID='{0}' AND ITEM_NO=2", strValue[0]));//���»�λ�������վ̨��
                     dal.UpdateTaskDetailCrane(strValue[3], "30" + strValue[4], "0", strValue[5], string.Format("TASK_ID='{0}' AND ITEM_NO=3", strValue[0]));//���µ��ȶѶ������ʵλ�ü�Ŀ���ַ��
                 }
             }

        }
        /// <summary>
        /// ��죬����������⣻
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSpotCheck_Click(object sender, EventArgs e)
        {
 
             string strTaskNo = ((string)Context.ProcessDispatcher.WriteToService("StockPLC_01", "01_1_195")).PadLeft(4,'0');

            string[] str = new string[3];
            if (int.Parse(strTaskNo) >= 9000 && int.Parse(strTaskNo) <= 9299) //����
                str[0] = "1";
            else if (int.Parse(strTaskNo) >= 9300 && int.Parse(strTaskNo) <= 9499)//���
                str[0] = "2";
            
            str[1] = "";
            str[2] = "";
            TaskDal dal = new TaskDal(); //��������ţ���ȡTaskID��BILL_NO
            string[] strInfo = dal.GetTaskInfo(strTaskNo);
            DataTable dt = dal.TaskInfo(string.Format("TASK_ID='{0}'", strInfo[0]));
            DataTable dtProductInfo = dal.GetProductInfoByTaskID(strInfo[0]);
            this.Stop(); //�߳�ֹͣ
            string strValue = "";
            while ((strValue = FormDialog.ShowDialog(str, dtProductInfo)) != "")
            {
                dal.UpdateTaskDetailState(string.Format("TASK_ID='{0}' AND ITEM_NO=2", strInfo[0]), "2");
                string writeItem = "01_2_195_";
                if (str[0] == "1" || str[0] == "2")  //��죬����
                {
                    dal.UpdateTaskState(strInfo[0], "2");

                    BillDal billdal = new BillDal();
                    billdal.UpdateBillMasterFinished(strInfo[1]);
                    Context.ProcessDispatcher.WriteToService("StockPLC_01", writeItem + "1", 1); //PLCд������
                }
                break;
            }
            this.Resume();

        }
        /// <summary>
        /// �̵�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckScan_Click(object sender, EventArgs e)
        {
            string strTaskNo = ((string)Context.ProcessDispatcher.WriteToService("StockPLC_01", "01_1_195")).PadLeft(4, '0');

            string[] str = new string[3];
           
            str[0] = "4";
           

            str[1] = "";
            str[2] = "";
            TaskDal dal = new TaskDal(); //��������ţ���ȡTaskID��BILL_NO
            string[] strInfo = dal.GetTaskInfo(strTaskNo);
            DataTable dt = dal.TaskInfo(string.Format("TASK_ID='{0}'", strInfo[0]));
            DataTable dtProductInfo = dal.GetProductInfoByTaskID(strInfo[0]);
            this.Stop(); //�߳�ֹͣ
            string strValue = "";
            while ((strValue = FormDialog.ShowDialog(str, dtProductInfo)) != "")
            {
                dal.UpdateTaskDetailState(string.Format("TASK_ID='{0}' AND ITEM_NO=2", strInfo[0]), "2");
                string writeItem = "01_2_195_";

                DataTable dtTask = dal.TaskInfo(string.Format("TASK_ID='{0}'", strInfo[0]));

                DataRow dr = dtTask.Rows[0];
                SysStationDal sysdal = new SysStationDal();
                DataTable dtstation = sysdal.GetSationInfo(dr["CELL_CODE"].ToString(), "11");

                if (strValue != "1")
                {
                    CellDal celldal = new CellDal();
                    celldal.UpdateCellNewPalletCode(dr["CELL_CODE"].ToString(), strValue);
                }


                int[] ServiceW = new int[3];
                ServiceW[0] = int.Parse(strInfo[1]); //�����
                ServiceW[1] = int.Parse(dtstation.Rows[0]["STATION_NO"].ToString());//Ŀ�ĵ�ַ
                ServiceW[2] = 1;

                Context.ProcessDispatcher.WriteToService("StockPLC_01", writeItem + "1", ServiceW); //PLCд������
                Context.ProcessDispatcher.WriteToService("StockPLC_01", writeItem + "3", 1); //PLCд������

                dal.UpdateTaskDetailStation("195", dtstation.Rows[0]["STATION_NO"].ToString(), "1", string.Format("TASK_ID='{0}' AND ITEM_NO=3", strInfo[0]));//���»�λ�������վ̨��
                dal.UpdateTaskDetailCrane(dtstation.Rows[0]["STATION_NO"].ToString(), dr["CELL_CODE"].ToString(), "0", dtstation.Rows[0]["CRANE_NO"].ToString(), string.Format("TASK_ID='{0}' AND ITEM_NO=4", strInfo[0]));//���µ��ȶѶ������ʵλ�ü�Ŀ���ַ��
                break;
            }
            this.Resume();
        }
    }
}
