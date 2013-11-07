using System;
using System.Xml;

namespace THOK.MCP.Service.Siemens.Config
{
	/// <summary>
	/// OPC�����ļ�������
	/// </summary>
	public class Configuration
	{
		private XmlDocument doc;

		private string connectionString;

		private string groupName;

		private string groupString;

		private int updateRate;

		private ItemInfo[] items;

        private string progid;
        private string servername;

		public string ConnectionString
		{
			get
			{
				return connectionString;
			}
		}
        public string ProgID
        {
            get
            {
                return progid;
            }
        }
        public string ServerName
        {
            get
            {
                return servername;
            }
        }

		public string GroupName
		{
			get
			{
				return groupName;
			}
		}

		public string GroupString
		{
			get
			{
				return groupString;
			}
		}

		public int UpdateRate
		{
			get
			{
				return updateRate;
			}
		}
		
		public ItemInfo[] Items
		{
			get
			{
				return items;
			}
		}

		public Configuration(string configFile)
		{
			doc = new XmlDocument();
			doc.Load(configFile);
			Initialize();
		}

		private void Initialize()
		{
			XmlNodeList nodeList = doc.GetElementsByTagName("OPCServer");
			if (nodeList.Count != 0)
			{
				connectionString = nodeList[0].Attributes["ConnectionString"].Value;
                string[] str = connectionString.Split(';');
                progid = str[0];
                servername = str[1];
			}
			else
			{
				throw new Exception("�������ļ����Ҳ�������OPCServer����Ϣ");
			}

			nodeList = doc.GetElementsByTagName("OPCGroup");

			if (nodeList.Count != 0)
			{
				groupName = nodeList[0].Attributes["GroupName"].Value;
				groupString = nodeList[0].Attributes["GroupString"].Value;
				updateRate = Convert.ToInt32(nodeList[0].Attributes["UpdateRate"].Value);
			}
			else
			{
				throw new Exception("�������ļ����Ҳ�������OPCGroup����Ϣ");
			}

			nodeList = doc.GetElementsByTagName("OPCItem");

			items = new ItemInfo[nodeList.Count];
			for (int i = 0; i < nodeList.Count; i++)
			{
				XmlNode node = nodeList[i];
				items[i] = new ItemInfo(node.Attributes["ItemName"].Value,
					groupString + node.Attributes["OPCItemName"].Value,
					Convert.ToInt32(node.Attributes["ClientHandler"].Value),
					node.Attributes["ItemType"].Value);
                if (node.Attributes["IsActive"] != null )
                {
                    items[i].IsActive = Convert.ToBoolean(node.Attributes["IsActive"].Value);
                }
			}
		}
	}
}
