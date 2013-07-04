using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
namespace THOK.MCP.Service.Sick
{
    public class BarcodeParse: IProtocolParse
    {
        private char headChar = (char)byte.Parse("02");
        private char tailChar = (char)byte.Parse("03");
        private string buffer = "";
        public static string separator = "*";

        public THOK.MCP.Message Parse(string msg)
        {
            THOK.MCP.Message message = null;

            buffer += msg;
            int headPos = buffer.IndexOf(headChar, 0);

            if (headPos != -1)
            {
                int tailPos = buffer.IndexOf(tailChar, headPos);
                if (tailPos != -1)
                {
                    if (headPos + 1 < tailPos)
                    {
                        //����ֹ��ǰ��������ʼ��,���Եڶ���Ϊ׼.
                        int tmp = buffer.IndexOf(this.headChar, (headPos + 1), (tailPos - headPos));
                        if (tmp >= 0)
                        {
                            headPos = headPos + tmp;
                        }

                        //��ȡλ�ڿ�ʼ�ͽ����ַ����е�������Ϣ
                        string scanData = buffer.Substring((headPos + 1), (tailPos - headPos - 1));
                        int pos = scanData.IndexOf(separator);
                        string command = "";
                        string barcode = "";
                        if (pos != -1)
                        {
                            command = scanData.Substring(0, pos).Trim();
                            barcode = scanData.Substring(pos + 1).Trim();

                            Dictionary<string, string> parameters = new Dictionary<string, string>();
                            parameters.Add("barcode", barcode);
                            message = new THOK.MCP.Message(true, msg, command, parameters);

                        }
                        else
                        {
                            message = new THOK.MCP.Message(msg);
                        }
                        //������������ȥ��
                        buffer = buffer.Remove(0, (tailPos + 1));
                        //������,ֻ��ȡһ������buffer
                        buffer = "";
                    }
                    else
                    {
                        buffer = buffer.Substring(headPos);
                    }
                }
                else
                    message = new THOK.MCP.Message(msg);
            }
            else
            {
                message = new THOK.MCP.Message(msg);
            }
            return message;
        }
    }
}
