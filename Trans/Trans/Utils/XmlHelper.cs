using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Trans.Entity;

namespace Trans.Utils
{
    public class XmlHelper
    {
        private Collection<DropListItem> list;


        public Collection<DropListItem> GetDropList()
        {
            if (list != null)
            {
                return list;
            }
            else
            {
                list = InitDropList();
                return list;
            }
        }

        private Collection<DropListItem> InitDropList()
        {
            list = new Collection<DropListItem>();
            XmlDocument doc = new XmlDocument();
            doc.Load(Environment.CurrentDirectory + @"\Configuration\BaseConfiguration.xml");

            XmlNode xmlRoot = doc.SelectSingleNode("dropitems");
            XmlNodeList xnl = xmlRoot.ChildNodes;

            DropListItem item;

            foreach (var i in xnl)
            {
                item = new DropListItem();
                XmlElement xe = (XmlElement)i;

                item.DisplayName = xe.GetAttribute("displayname");
                item.Code = xe.GetAttribute("code");
                list.Add(item);
            }
            return list;
        }
    }
}
