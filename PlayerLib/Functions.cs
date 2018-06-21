using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
//using Emitters;
//using CLikeLang;
using CLL;

namespace PlayerLib
{
    partial class Game
    {

        public override void CallFunction(string name)
        {
            
                if (!functions.Keys.Contains(name))
                    throw new Exception("Trying to execute unkown function " + name);
            try
            {
                functions[name].Execute(this);
            }
            catch (Exception e)
            {
                throw new Exception("Error running function " + name, e);
            }

        }

        void BuildFunctions(XmlDocument doc)
        {
            events.Clear();
            functions.Clear();
            
            XmlNodeList evts = doc.SelectNodes("//project/events/event");


            foreach (XmlNode n in evts)
            {
                string name="";
                try
                {

                    name = n.Attributes.GetNamedItem("name").Value;
                    string code = n.InnerText;
                    events.Add(CLL.FunctionBuilder.BuildFunction(this, name, code));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in event " + name + ":" + ex.Message);
                }
            }

            evts = doc.SelectNodes("//project/routines/routine");

            foreach (XmlNode n in evts)
            {
                CLL.FunctionBuilder fb = new CLL.FunctionBuilder();

                string name = n.Attributes.GetNamedItem("name").Value;
                string code = n.InnerText;
                CLL.Function f = CLL.FunctionBuilder.BuildFunction(this, name, code);
                functions.Add(name, f);
            }
        }
        
    }
}
