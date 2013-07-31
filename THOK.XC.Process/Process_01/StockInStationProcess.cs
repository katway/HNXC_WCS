﻿using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using System.Data;
using THOK.XC.Process.Dal;

namespace THOK.XC.Process.Process_01
{
    public class StockInStationProcess : AbstractProcess
    {
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            /*  
             * 一楼入库到达入库站台。
            */
            try
            {
                switch (stateItem.ItemName)
                {
                    case "01_1_136":
                        break;
                    case "01_1_148":
                        break;
                    case "01_1_152":
                        break;
                    case "01_1_170":
                        break;
                    case "01_1_178":
                        break;
                    case "01_1_186":
                        break;
                }

                string TaskNo = ""; //读取PLC任务号。
                TaskDal taskDal = new TaskDal();
                string strWhere = string.Format("TASK_NO='{0}' AND TASK_TYPE='11' AND DETAIL.STATE='0'", TaskNo);
                DataTable dtInCrane = taskDal.TaskCraneDetail(strWhere);
                if (dtInCrane.Rows.Count > 0)
                    WriteToService("Crane", "CraneInRequest", dtInCrane);
            }
            catch (Exception e)
            {
                Logger.Error("入库任务请求批次生成处理失败，原因：" + e.Message);
            }
        }
    }
}
