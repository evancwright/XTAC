/*This class reads a Trizbort XML file and loads it into a Game object
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using TrizbortXml;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace XTAC
{
    class TrizbortImporter
    {

        public void Import(Xml project, string fileName)
        {
            Trizbort triz;
            Dictionary<int, int> idToIndex = new Dictionary<int,int>();
                
            System.Xml.Serialization.XmlSerializer reader = new XmlSerializer(typeof(Trizbort));

            // Read the XML file.
            StreamReader file = new StreamReader(fileName);
             
            // Deserialize the content of the file into a Book object.
            triz = (Trizbort)reader.Deserialize(file);
            
            file.Close();

            project.Project.Objects.Object.RemoveAt(2);  // remove default room
            int index = 2; //start index
            foreach (Room r in triz.Map.Room)
            {
                int _id = Convert.ToInt32(r.Id);
                idToIndex[_id] =  index;
                index++;
            }
            

            int id=2;
            //convert the file into a Xml project
            foreach (Room r in triz.Map.Room)
            {
                    Object obj = CreateObject();

                    obj.Name = r.Name;

                    if (r.Name.Equals(""))
                    {
                        obj.Name = "Unamed Room";
                    }

                    obj.Id = "" + id;
                    obj.Description = r.Description;

                    if (obj.Description.Length == 0)
                    {
                        obj.Description = "DESCRIPTION NOT SET";
                    }

                    obj.Holder = "0";
                    //obj.Flags.Emittinglight = r.Objects.
                    obj.Flags.Emittinglight = "1";
                    obj.Initialdescription = "";
                    obj.Synonyms = new Synonyms();
                    obj.Synonyms.Names = "";

                    project.Project.Objects.Object.Add(obj);
                    ++id;

            }

            //create the connections between rooms
            int i = 3;
            foreach (Line l in triz.Map.Line)
            {
                try
                {
                    int id1;
                    int id2;

                    id1 = Convert.ToInt32(l.Dock[0].Id);
                    id2 = Convert.ToInt32(l.Dock[1].Id);


                    int index1 = idToIndex[id1];
                    int index2 = idToIndex[id2];

                    Object r1 = project.Project.Objects.Object.ElementAt(index1); //convert triz ids to internal ids
                    Object r2 = project.Project.Objects.Object.ElementAt(index2);


                    SetRoomConnection(r1, l.Dock[0].Port, index2);

                    if (l.flow != null)
                    {
                        if (!l.flow.Equals("oneWay"))
                        {
                            SetRoomConnection(r2, l.Dock[1].Port, index1);
                        }
                    }
                    else
                    {
                        SetRoomConnection(r2, l.Dock[1].Port, index1 );
                    }
                }
                catch (Exception ex)
                {
                //    throw new Exception("Import failed", ex);
                }
                      


            }
           

            //create object entries for the objects in each room
            //create the connections between rooms
            i = 2;

            foreach (Room r in triz.Map.Room)
            {
                string objects = r.Objects;
                if (objects != null)
                {
                    string[] objs = objects.Split('|');

                    foreach (string s in objs)
                    {
                        if (!s.Equals(""))
                        {
                            Object thing = CreateObject();
                            thing.Name = s.Trim();
                            thing.Id = "" + id;
                            thing.Description = "YOU NOTICE NOTHING UNEXPECTED.";
                            thing.Initialdescription = "";
                            thing.Synonyms = new Synonyms();
                            thing.Synonyms.Names = "";
                            thing.Holder = "" + i;
                            project.Project.Objects.Object.Add(thing);
                            id++;
                        }
                    }
                }
                i++;
            }

            
        }

        Object CreateObject()
        {
                Object obj = new Object();
                obj.Directions = new Directions();
                obj.Flags = new Flags();
                obj.Nogo = new Nogo();
                obj.Holder = "offscreen";
                
                obj.Directions.N = "255";
                obj.Directions.S = "255";
                obj.Directions.E = "255";
                obj.Directions.W = "255";
                obj.Directions.Ne = "255";
                obj.Directions.Nw = "255";
                obj.Directions.Se = "255";
                obj.Directions.Sw = "255";
                obj.Directions.Up = "255";
                obj.Directions.Down = "255"; 
                obj.Directions.In = "255";
                obj.Directions.Out = "255";
                obj.Directions.Mass = "0";
                return obj;
        }

        void SetRoomConnection(Object room1, string direction, int rid)
        {
            
            if (direction == "n")
                room1.Directions.N = ""+rid;
            else if (direction == "s")
                room1.Directions.S = "" + rid;
            else if (direction == "e")
                room1.Directions.E = "" + rid;
            else if (direction == "w")
                room1.Directions.W = "" + rid;
            else if (direction == "ne")
                room1.Directions.Ne = "" + rid;
            else if (direction == "se")
                room1.Directions.Se = "" +rid;
            else if (direction == "Ne")
                room1.Directions.Ne = "" + rid;
            else if (direction == "nw")
                room1.Directions.Nw = "" + rid;
            else if (direction == "in")
                room1.Directions.In = "" + rid;
            else if (direction == "out")
                room1.Directions.Out = ""+rid;
            else if (direction == "up")
                room1.Directions.Up = "" + rid;
            else if (direction == "down")
                room1.Directions.Down = "" + rid;

        }

    }
}
