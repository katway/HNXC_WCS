using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.XC.Process.Dao;
using THOK.Util;

namespace THOK.XC.Process.Dal
{
    public class ChannelDal : BaseDal
    {

        /// <summary>
        /// ���仺��������뻺�浽�������ػ����ID��
        /// </summary>
        /// <param name="TaskID"></param>
        /// <returns></returns>
        public string InsertChannel(string TaskID,string Bill_No)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                string strChannel_No = "";
                TaskDao dao = new TaskDao();
                DataTable dt = dao.TaskInfo(string.Format("TASK_ID='{0}'", TaskID));
                string Line_No = dt.Rows[0]["TARGET_CODE"].ToString().Trim();
                string BillNo = dt.Rows[0]["BILL_NO"].ToString();

                ChannelDao Cdao = new ChannelDao();
                dt = Cdao.ChannelInfo(Line_No);

                if (!Cdao.HasTaskInChannel(TaskID))
                {
                    switch (Line_No)
                    {
                        case "01":
                            DataRow dr011 = dt.Rows[0];
                            DataRow dr012 = dt.Rows[1];
                            DataRow dr013 = dt.Rows[2];
                            if (dr011["QTY"].ToString() == "0")
                            {
                                if (dr012["QTY"].ToString() == "0")
                                {
                                    if (dr013["QTY"].ToString() == "0")
                                    {
                                        strChannel_No = dr011["CHANNEL_NO"].ToString();
                                    }
                                    else if (int.Parse(dr013["CACHE_QTY"].ToString()) - int.Parse(dr013["QTY"].ToString()) > 0)
                                    {
                                        DataTable dt013 = Cdao.ChannelProductInfo(dr013["CHANNEL_NO"].ToString());
                                        if (dt013.Rows.Count > 0)
                                        {
                                            if (dt013.Rows[0]["BILL_NO"].ToString() == BillNo)
                                            {
                                                strChannel_No = dr013["CHANNEL_NO"].ToString();
                                            }
                                            else
                                            {
                                                strChannel_No = dr011["CHANNEL_NO"].ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        strChannel_No = dr011["CHANNEL_NO"].ToString();
                                    }
                                }
                                else if (int.Parse(dr012["CACHE_QTY"].ToString()) - int.Parse(dr012["QTY"].ToString()) > 0)
                                {
                                    DataTable dt012 = Cdao.ChannelProductInfo(dr012["CHANNEL_NO"].ToString());
                                    if (dt012.Rows.Count > 0)
                                    {
                                        if (dt012.Rows[0]["BILL_NO"].ToString() == BillNo)
                                        {
                                            strChannel_No = dr012["CHANNEL_NO"].ToString();
                                        }
                                        else
                                        {
                                            if (dr013["QTY"].ToString() == "0")
                                            {
                                                strChannel_No = dr011["CHANNEL_NO"].ToString();
                                            }
                                            else if (int.Parse(dr013["CACHE_QTY"].ToString()) - int.Parse(dr013["QTY"].ToString()) > 0)
                                            {
                                                DataTable dt013 = Cdao.ChannelProductInfo(dr013["CHANNEL_NO"].ToString());
                                                if (dt013.Rows.Count > 0)
                                                {
                                                    if (dt013.Rows[0]["BILL_NO"].ToString() == BillNo)
                                                    {
                                                        strChannel_No = dr013["CHANNEL_NO"].ToString();
                                                    }
                                                    else
                                                    {
                                                        strChannel_No = dr011["CHANNEL_NO"].ToString();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                strChannel_No = dr011["CHANNEL_NO"].ToString();
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (dr013["QTY"].ToString() == "0")
                                    {
                                        strChannel_No = dr011["CHANNEL_NO"].ToString();
                                    }
                                    else if (int.Parse(dr013["CACHE_QTY"].ToString()) - int.Parse(dr013["QTY"].ToString()) > 0)
                                    {
                                        DataTable dt013 = Cdao.ChannelProductInfo(dr013["CHANNEL_NO"].ToString());
                                        if (dt013.Rows.Count > 0)
                                        {
                                            if (dt013.Rows[0]["BILL_NO"].ToString() == BillNo)
                                            {
                                                strChannel_No = dr013["CHANNEL_NO"].ToString();
                                            }
                                            else
                                            {
                                                strChannel_No = dr011["CHANNEL_NO"].ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        strChannel_No = dr011["CHANNEL_NO"].ToString();
                                    }
                                }


                            }
                            else if (int.Parse(dr011["CACHE_QTY"].ToString()) - int.Parse(dr011["QTY"].ToString()) > 0)
                            {
                                DataTable dt011 = Cdao.ChannelProductInfo(dr011["CHANNEL_NO"].ToString());
                                if (dt011.Rows.Count > 0)
                                {
                                    if (dt011.Rows[0]["BILL_NO"].ToString() == BillNo)
                                    {
                                        strChannel_No = dr011["CHANNEL_NO"].ToString();
                                    }
                                    else
                                    {
                                        if (dr012["QTY"].ToString() == "0")
                                        {
                                            if (dr013["QTY"].ToString() == "0")
                                            {
                                                strChannel_No = dr012["CHANNEL_NO"].ToString();
                                            }
                                            else if (int.Parse(dr013["CACHE_QTY"].ToString()) - int.Parse(dr013["QTY"].ToString()) > 0)
                                            {
                                                DataTable dt013 = Cdao.ChannelProductInfo(dr013["CHANNEL_NO"].ToString());
                                                if (dt013.Rows.Count > 0)
                                                {
                                                    if (dt013.Rows[0]["BILL_NO"].ToString() == BillNo)
                                                    {
                                                        strChannel_No = dr013["CHANNEL_NO"].ToString();
                                                    }
                                                    else
                                                    {
                                                        strChannel_No = dr012["CHANNEL_NO"].ToString();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                strChannel_No = dr012["CHANNEL_NO"].ToString();
                                            }
                                        }
                                        else if (int.Parse(dr012["CACHE_QTY"].ToString()) - int.Parse(dr012["QTY"].ToString()) > 0)
                                        {
                                            DataTable dt012 = Cdao.ChannelProductInfo(dr012["CHANNEL_NO"].ToString());
                                            if (dt012.Rows.Count > 0)
                                            {
                                                if (dt012.Rows[0]["BILL_NO"].ToString() == BillNo)
                                                {
                                                    strChannel_No = dr012["CHANNEL_NO"].ToString();
                                                }
                                                else
                                                {
                                                    if (dr013["QTY"].ToString() == "0")
                                                    {
                                                        strChannel_No = dr013["CHANNEL_NO"].ToString();
                                                    }
                                                    else if (int.Parse(dr013["CACHE_QTY"].ToString()) - int.Parse(dr013["QTY"].ToString()) > 0)
                                                    {
                                                        DataTable dt013 = Cdao.ChannelProductInfo(dr013["CHANNEL_NO"].ToString());
                                                        if (dt013.Rows.Count > 0)
                                                        {
                                                            if (dt013.Rows[0]["BILL_NO"].ToString() == BillNo)
                                                            {
                                                                strChannel_No = dr013["CHANNEL_NO"].ToString();
                                                            }
                                                        }
                                                    }

                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (dr013["QTY"].ToString() == "0")
                                            {
                                                strChannel_No = dr013["CHANNEL_NO"].ToString();
                                            }
                                            else if (int.Parse(dr013["CACHE_QTY"].ToString()) - int.Parse(dr013["QTY"].ToString()) > 0)
                                            {
                                                DataTable dt013 = Cdao.ChannelProductInfo(dr013["CHANNEL_NO"].ToString());
                                                if (dt013.Rows.Count > 0)
                                                {
                                                    if (dt013.Rows[0]["BILL_NO"].ToString() == BillNo)
                                                    {
                                                        strChannel_No = dr013["CHANNEL_NO"].ToString();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (dr012["QTY"].ToString() == "0")
                                {
                                    if (dr013["QTY"].ToString() == "0")
                                    {
                                        strChannel_No = dr012["CHANNEL_NO"].ToString();
                                    }
                                    else if (int.Parse(dr013["CACHE_QTY"].ToString()) - int.Parse(dr013["QTY"].ToString()) > 0)
                                    {
                                        DataTable dt013 = Cdao.ChannelProductInfo(dr013["CHANNEL_NO"].ToString());
                                        if (dt013.Rows.Count > 0)
                                        {
                                            if (dt013.Rows[0]["BILL_NO"].ToString() == BillNo)
                                            {
                                                strChannel_No = dr013["CHANNEL_NO"].ToString();
                                            }
                                            else
                                            {
                                                strChannel_No = dr012["CHANNEL_NO"].ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        strChannel_No = dr012["CHANNEL_NO"].ToString();
                                    }
                                }
                                else if (int.Parse(dr012["CACHE_QTY"].ToString()) - int.Parse(dr012["QTY"].ToString()) > 0)
                                {
                                    DataTable dt012 = Cdao.ChannelProductInfo(dr012["CHANNEL_NO"].ToString());
                                    if (dt012.Rows.Count > 0)
                                    {
                                        if (dt012.Rows[0]["BILL_NO"].ToString() == BillNo)
                                        {
                                            strChannel_No = dr012["CHANNEL_NO"].ToString();
                                        }
                                        else
                                        {
                                            if (dr013["QTY"].ToString() == "0")
                                            {
                                                strChannel_No = dr013["CHANNEL_NO"].ToString();
                                            }
                                            else if (int.Parse(dr013["CACHE_QTY"].ToString()) - int.Parse(dr013["QTY"].ToString()) > 0)
                                            {
                                                DataTable dt013 = Cdao.ChannelProductInfo(dr013["CHANNEL_NO"].ToString());
                                                if (dt013.Rows.Count > 0)
                                                {
                                                    if (dt013.Rows[0]["BILL_NO"].ToString() == BillNo)
                                                    {
                                                        strChannel_No = dr013["CHANNEL_NO"].ToString();
                                                    }
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (dr012["QTY"].ToString() == "0")
                                {
                                    if (dr013["QTY"].ToString() == "0")
                                    {
                                        strChannel_No = dr012["CHANNEL_NO"].ToString();
                                    }
                                    else if (int.Parse(dr013["CACHE_QTY"].ToString()) - int.Parse(dr013["QTY"].ToString()) > 0)
                                    {
                                        DataTable dt013 = Cdao.ChannelProductInfo(dr013["CHANNEL_NO"].ToString());
                                        if (dt013.Rows.Count > 0)
                                        {
                                            if (dt013.Rows[0]["BILL_NO"].ToString() == BillNo)
                                            {
                                                strChannel_No = dr013["CHANNEL_NO"].ToString();
                                            }
                                            else
                                            {
                                                strChannel_No = dr012["CHANNEL_NO"].ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        strChannel_No = dr012["CHANNEL_NO"].ToString();
                                    }
                                }
                                else if (int.Parse(dr012["CACHE_QTY"].ToString()) - int.Parse(dr012["QTY"].ToString()) > 0)
                                {
                                    DataTable dt012 = Cdao.ChannelProductInfo(dr012["CHANNEL_NO"].ToString());
                                    if (dt012.Rows.Count > 0)
                                    {
                                        if (dt012.Rows[0]["BILL_NO"].ToString() == BillNo)
                                        {
                                            strChannel_No = dr012["CHANNEL_NO"].ToString();
                                        }
                                        else
                                        {
                                            if (dr013["QTY"].ToString() == "0")
                                            {
                                                strChannel_No = dr013["CHANNEL_NO"].ToString();
                                            }
                                            else if (int.Parse(dr013["CACHE_QTY"].ToString()) - int.Parse(dr013["QTY"].ToString()) > 0)
                                            {
                                                DataTable dt013 = Cdao.ChannelProductInfo(dr013["CHANNEL_NO"].ToString());
                                                if (dt013.Rows.Count > 0)
                                                {
                                                    if (dt013.Rows[0]["BILL_NO"].ToString() == BillNo)
                                                    {
                                                        strChannel_No = dr013["CHANNEL_NO"].ToString();
                                                    }
                                                }
                                            }

                                        }
                                    }

                                }
                                else
                                {
                                    if (dr013["QTY"].ToString() == "0")
                                    {
                                        strChannel_No = dr013["CHANNEL_NO"].ToString();
                                    }
                                    else if (int.Parse(dr013["CACHE_QTY"].ToString()) - int.Parse(dr013["QTY"].ToString()) > 0)
                                    {
                                        DataTable dt013 = Cdao.ChannelProductInfo(dr013["CHANNEL_NO"].ToString());
                                        if (dt013.Rows.Count > 0)
                                        {
                                            if (dt013.Rows[0]["BILL_NO"].ToString() == BillNo)
                                            {
                                                strChannel_No = dr013["CHANNEL_NO"].ToString();
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        case "02":
                            DataRow dr021 = dt.Rows[0];
                            DataRow dr022 = dt.Rows[1];
                            if (dr021["QTY"].ToString() == "0")
                            {
                                if (dr022["QTY"].ToString() == "0")
                                {
                                    strChannel_No = dr021["CHANNEL_NO"].ToString();
                                }
                                else if (int.Parse(dr022["CACHE_QTY"].ToString()) - int.Parse(dr022["QTY"].ToString()) > 0)
                                {
                                    DataTable dt022 = Cdao.ChannelProductInfo(dr022["CHANNEL_NO"].ToString());
                                    if (dt022.Rows.Count > 0)
                                    {
                                        if (dt022.Rows[0]["BILL_NO"].ToString() == BillNo)
                                        {
                                            strChannel_No = dr022["CHANNEL_NO"].ToString();
                                        }
                                        else
                                        {
                                            strChannel_No = dr021["CHANNEL_NO"].ToString();
                                        }
                                    }
                                }

                                else
                                {
                                    strChannel_No = dr021["CHANNEL_NO"].ToString();
                                }

                            }
                            else if (int.Parse(dr021["CACHE_QTY"].ToString()) - int.Parse(dr021["QTY"].ToString()) > 0)
                            {
                                DataTable dt021 = Cdao.ChannelProductInfo(dr021["CHANNEL_NO"].ToString());
                                DataTable dt022 = Cdao.ChannelProductInfo(dr022["CHANNEL_NO"].ToString());

                                if (dt021.Rows.Count > 0)
                                {
                                    if (dt021.Rows[0]["BILL_NO"].ToString() == BillNo)
                                    {
                                        strChannel_No = dr021["CHANNEL_NO"].ToString();
                                    }
                                    else
                                    {
                                        if (int.Parse(dr022["QTY"].ToString()) == 0)
                                        {
                                            strChannel_No = dr022["CHANNEL_NO"].ToString();
                                        }
                                        else
                                        {
                                            if (int.Parse(dr022["CACHE_QTY"].ToString()) - int.Parse(dr022["QTY"].ToString()) > 0)
                                            {
                                                if (dt022.Rows[0]["BILL_NO"].ToString() == BillNo)
                                                {
                                                    strChannel_No = dr022["CHANNEL_NO"].ToString();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                DataTable dt022 = Cdao.ChannelProductInfo(dr022["CHANNEL_NO"].ToString());
                                if (int.Parse(dr022["QTY"].ToString()) == 0)
                                {
                                    strChannel_No = dr022["CHANNEL_NO"].ToString();
                                }
                                else
                                {
                                    if (int.Parse(dr022["CACHE_QTY"].ToString()) - int.Parse(dr022["QTY"].ToString()) > 0)
                                    {
                                        if (dt022.Rows[0]["BILL_NO"].ToString() == BillNo)
                                        {
                                            strChannel_No = dr022["CHANNEL_NO"].ToString();
                                        }
                                    }
                                }
                            }


                            break;
                        case "03":
                            if (int.Parse(dt.Rows[0]["CACHE_QTY"].ToString()) - int.Parse(dt.Rows[0]["QTY"].ToString()) > 15)
                            {
                                strChannel_No = dt.Rows[0]["CHANNEL_NO"].ToString();
                            }
                            break;
                    }

                    if (strChannel_No != "")
                    {
                        Cdao.InsertChannel(TaskID, Bill_No, strChannel_No);
                    }
                }

                return strChannel_No;
            }
        }

        /// <summary>
        /// ���½��뻺���ʱ�䣬��ORDER_NO     
        /// </summary>
        /// <returns></returns>
        public int UpdateInChannelTime(string TaskID, string Bill_No, string ChannelNo)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                int strValue = 0;
                ChannelDao dao = new ChannelDao();
                int count = dao.ProductCount(Bill_No);
                TaskDao tdao = new TaskDao();

                int taskCount = tdao.TaskCount(Bill_No);
                if (count == 0)
                    strValue = 1;
                if (count == taskCount - 1)
                    strValue = 2;
                dao.UpdateInChannelTime(TaskID, Bill_No, ChannelNo);
                return strValue;
            }
        }
        /// <summary>
        /// �ж��Ƿ��Ѿ��ڻ�����У�true ����
        /// </summary>
        /// <param name="TaskID"></param>
        /// <returns></returns>
        public bool HasTaskInChannel(string TaskID)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                bool blnValue = false;
                 ChannelDao dao = new ChannelDao();
                 blnValue= dao.HasTaskInChannel(TaskID);
                 return blnValue;
            }
 
        }
          /// <summary>
        /// ���³���
        /// </summary>
        public void UpdateOutChannelTime(string TaskID)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                ChannelDao dao = new ChannelDao();
                dao.UpdateOutChannelTime(TaskID);
            }
        }
        /// <summary>
        /// ��ȡ����Ļ����
        /// </summary>
        /// <param name="TaskNO"></param>
        /// <param name="BillNo"></param>
        /// <returns></returns>
        public string GetChannelFromTask(string TaskNO, string BillNo)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                ChannelDao dao = new ChannelDao();
                return dao.GetChannelFromTask(TaskNO, BillNo);
            }
        }

        /// <summary>
        /// ���³���
        /// </summary>
        public int UpdateInChannelAndTime(string TaskID, string Bill_No, string ChannelNo)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                int strValue = 0;
                ChannelDao dao = new ChannelDao();
                int count = dao.ProductCount(Bill_No);
                TaskDao tdao = new TaskDao();

                int taskCount = tdao.TaskCount(Bill_No);
                if (count == 0)
                    strValue = 1;
                if (count == taskCount - 1)
                    strValue = 2;
                dao.UpdateInChannelAndTime(TaskID, Bill_No, ChannelNo);
                return strValue;
            }
        }
       
    }
}
