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
                THOK.XC.Process.Util.LogFile.DeleteFile();
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

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DownloadData();
            try
            {
                Context.ProcessDispatcher.WriteToProcess("LEDProcess", "Refresh", null);
                Context.ProcessDispatcher.WriteToProcess("LedStateProcess", "Refresh", null);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            string text = "�ֹ����¾���������Ϣ��";
            string cigaretteCode = "";
            string barcode = "";

            Scan(text, cigaretteCode, barcode);
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
            btnDownload.Enabled = !isStart;
            btnUpload.Enabled = !isStart;
            btnStart.Enabled = !isStart;
            btnStop.Enabled = isStart;
            btnSimulate.Enabled = !isStart;
        }

        private void btnSimulate_Click(object sender, EventArgs e)
        {            
            try
            {
                StockOutDal dal = new StockOutDal();
                dal.ClearNoScanData();
            }
            catch (Exception ee)
            {
                Logger.Error("���PLCδɨ�������Ϣ����ʧ�ܣ�ԭ��" + ee.Message);
            }
        }

        /// <summary>
        /// �������� ����޸����� 2010-10-30
        /// </summary>
        private void DownloadData()
        {
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    ChannelDal channelDal = new ChannelDal();
                    StockOutBatchDal stockOutBatchDal = new StockOutBatchDal();
                    StockInBatchDal stockInBatchDal = new StockInBatchDal();
                    StockOutDal stockOutDal = new StockOutDal();
                    StockInDal stockInDal = new StockInDal();
                    SupplyDal supplyDal = new SupplyDal();

                    if (supplyDal.FindCount() != stockOutDal.FindOutQuantity())
                        if (DialogResult.Cancel == MessageBox.Show("����δ��������ݣ���ȷ��Ҫ��������������", "ѯ��", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                            return;

                    using (PersistentManager pmServer = new PersistentManager("ServerConnection"))
                    {
                        ServerDal serverDal = new ServerDal();
                        serverDal.SetPersistentManager(pmServer);

                        //ORDER BY ORDERDATE,BATCHNO  ���ҵ�һ���Σ��������Ż��������ϴ�һ�Ź��̣�δ���ص����Σ�
                        DataTable table = serverDal.FindBatch();
                        if (table.Rows.Count != 0)
                        {
                            using (PersistentManager pmWES = new PersistentManager("WESConnection"))
                            {
                                StockInBatchDal stockInBatchDalWES = new StockInBatchDal();
                                stockInBatchDalWES.SetPersistentManager(pmWES);
                                stockInBatchDalWES.Delete();
                            }
                            
                            string batchID = table.Rows[0]["BATCHID"].ToString();
                            string orderDate = table.Rows[0]["ORDERDATE"].ToString();
                            string batchNo = table.Rows[0]["BATCHNO"].ToString();

                            Context.ProcessDispatcher.WriteToProcess("monitorView", "ProgressState", new ProgressState("�����ҵ��", 5, 1));
                            channelDal.Delete();
                            stockOutBatchDal.Delete();
                            stockOutDal.Delete();
                            stockInBatchDal.Delete();
                            stockInDal.Delete();
                            supplyDal.Delete();
                            System.Threading.Thread.Sleep(100);

                            Context.ProcessDispatcher.WriteToProcess("monitorView", "ProgressState", new ProgressState("���ز����̵���", 5, 2));
                            table = serverDal.FindStockChannel(orderDate, batchNo);
                            channelDal.InsertChannel(table);
                            System.Threading.Thread.Sleep(100);

                            Context.ProcessDispatcher.WriteToProcess("monitorView", "ProgressState", new ProgressState("���ز�������̵���", 5, 3));
                            table = serverDal.FindMixChannel(orderDate, batchNo);
                            channelDal.InsertMixChannel(table);
                            System.Threading.Thread.Sleep(100);

                            Context.ProcessDispatcher.WriteToProcess("monitorView", "ProgressState", new ProgressState("���طּ��̵���", 5, 4));
                            table = serverDal.FindChannelUSED(orderDate, batchNo);
                            channelDal.InsertChannelUSED(table);
                            System.Threading.Thread.Sleep(100);

                            Context.ProcessDispatcher.WriteToProcess("monitorView", "ProgressState", new ProgressState("���ز����ƻ���", 5, 5));
                            table = serverDal.FindSupply(orderDate, batchNo);
                            supplyDal.Insert(table);
                            System.Threading.Thread.Sleep(100);

                            serverDal.UpdateBatchStatus(batchID);
                            Context.ProcessDispatcher.WriteToProcess("monitorView", "ProgressState", new ProgressState()); 
                            Logger.Info("�����������");

                            //��ʼ��PLC���ݣ�������PLC��������PLC��
                            Context.ProcessDispatcher.WriteToService("StockPLC_01", "RestartData", 3);
                            Context.ProcessDispatcher.WriteToService("StockPLC_02", "RestartData", 1);

                            //��ʼ�����ɨ����
                            Context.ProcessDispatcher.WriteToProcess("ScanProcess", "Init", null);

                            //��ʼ��״̬������
                            Context.ProcessDispatcher.WriteToProcess("LedStateProcess", "Init", null);
                            Context.ProcessDispatcher.WriteToProcess("OrderDataStateProcess", "Init", null);
                            Context.ProcessDispatcher.WriteToProcess("ScannerStateProcess", "Init", null);

                            //�������������������
                            Context.ProcessDispatcher.WriteToProcess("StockInRequestProcess", "FirstBatch", null);
                            //���ɲ���������������
                            //Context.ProcessDispatcher.WriteToProcess("SupplyFirstRequestProcess", "FirstBatch", null);                   
                        }
                        else
                            MessageBox.Show("û�в����ƻ����ݣ�", "��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("�������ش���ʧ�ܣ�ԭ��" + e.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TaskDal taskDal = new TaskDal();
            DataTable dt = taskDal.TaskOutToDetail();
            DataTable dt2 = null;
            if (IndexStar == 0)
            {
                dt2 = taskDal.TaskCraneDetail("11,21,12,22", "1,2");
            }
            DataTable[] dtSend = new DataTable[2];
            dtSend[0] = dt;
            dtSend[1] = dt2;
            Context.ProcessDispatcher.WriteToProcess("StockInRequestProcess", "StockInRequest", dtSend);
            IndexStar++;
        }


        public delegate void ProcessStateInMainThread(StateItem stateItem);
        private void ProcessState(StateItem stateItem)
        {
            switch (stateItem.ItemName)
            {
                case "SimulateDialog":
                    string scannerCode = stateItem.State.ToString();
                    THOK.XC.Dispatching.View.SimulateDialog simulateDialog = new THOK.XC.Dispatching.View.SimulateDialog();
                    simulateDialog.Text = scannerCode + " ��ɨ�����ֹ�ɨ�룡";
                    if (simulateDialog.ShowDialog() == DialogResult.OK)
                    {
                        Dictionary<string, string>  parameters = new Dictionary<string, string>();
                        parameters.Add("barcode", simulateDialog.Barcode);                        
                        Context.ProcessDispatcher.WriteToProcess("ScanProcess", scannerCode, parameters);
                    }
                    Context.ProcessDispatcher.WriteToProcess("ScanProcess","ErrReset", "01");
                    break;
                case "ScanDialog":
                    Dictionary<string, string> scanParam = (Dictionary<string, string>)stateItem.State;
                    Scan(scanParam["text"], scanParam["cigaretteCode"], scanParam["barcode"]);
                    break;
                case "MessageBox":
                    Dictionary<string, object> msgParam = (Dictionary<string, object>)stateItem.State;
                    MessageBox.Show((string)msgParam["msg"], (string)msgParam["title"], (MessageBoxButtons)msgParam["messageBoxButtons"], (MessageBoxIcon)msgParam["messageBoxIcon"]);
                    break;
                default:
                    break;
            }
        }

        public void Scan(string text, string cigaretteCode, string barcode)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                StockOutDal outDal = new StockOutDal();
                SupplyDal supplyDal = new SupplyDal();

                if (barcode != string.Empty && supplyDal.Exist(barcode))
                    return;

                DataTable table = supplyDal.FindCigaretteAll(cigaretteCode);

                if (table.Rows.Count > 0)
                {
                    THOK.XC.Dispatching.View.ScanDialog scanDialog = new THOK.XC.Dispatching.View.ScanDialog(table);
                    scanDialog.setInformation(text, barcode);
                    if (scanDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (scanDialog.IsPass && scanDialog.Barcode.Length == 6)
                        {
                            cigaretteCode = scanDialog.SelectedCigaretteCode;
                            barcode = scanDialog.Barcode;

                            using (PersistentManager pmServer = new PersistentManager("ServerConnection"))
                            {
                                ServerDal serverDal = new ServerDal();
                                serverDal.SetPersistentManager(pmServer);
                                serverDal.UpdateCigaretteToServer(barcode, cigaretteCode);
                            }
                            outDal.UpdateCigarette(barcode, cigaretteCode);
                        }
                        else
                        {
                            MessageBox.Show("��֤�����", "��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        public override void Process(StateItem stateItem)
        {
            base.Process(stateItem);
            this.BeginInvoke(new ProcessStateInMainThread(ProcessState), stateItem);
        }
    }
}
