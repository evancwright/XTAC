using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PlayerLib
{
    
     

    partial class Game
    {
        delegate bool VerbCheckDlgt();

        List<Tuple<int,string>> checkList = new List<Tuple<int,string>>();
        Dictionary<string, VerbCheckDlgt> checkTable = new Dictionary<string, VerbCheckDlgt>();

        void BuildCheckTable(XmlDocument doc)
        {
            checkTable.Clear();

            //create jump table
            checkTable.Add("check_see_dobj", new VerbCheckDlgt(check_see_dobj));
            checkTable.Add("check_have_dobj", new VerbCheckDlgt(check_have_dobj));
            checkTable.Add("check_have_iobj", new VerbCheckDlgt(check_have_iobj));
            checkTable.Add("check_dobj_portable", new VerbCheckDlgt(check_dobj_portable));
            checkTable.Add("check_dobj_open", new VerbCheckDlgt(check_dobj_open));
            checkTable.Add("check_dobj_closed", new VerbCheckDlgt(check_dobj_closed));
            checkTable.Add("check_dobj_opnable", new VerbCheckDlgt(check_dobj_opnable));
            checkTable.Add("check_dobj_supplied", new VerbCheckDlgt(check_dobj_supplied));
            checkTable.Add("check_iobj_supplied", new VerbCheckDlgt(check_iobj_supplied));
            checkTable.Add("check_prep_supplied", new VerbCheckDlgt(check_prep_supplied));
            checkTable.Add("check_dobj_unlocked", new VerbCheckDlgt(check_dobj_unlocked));
            checkTable.Add("check_dont_have_dobj", new VerbCheckDlgt(check_dont_have_dobj));
            checkTable.Add("check_light", new VerbCheckDlgt(check_light));
            checkTable.Add("check_not_self_or_child", new VerbCheckDlgt(check_not_self_or_child));
            
            XmlNodeList checks = doc.SelectNodes("//project/checks/check");

            foreach (XmlNode c in checks)
            {
                string verb = c.Attributes.GetNamedItem("verb").Value;

                if (verb.IndexOf(",") != -1)
                {//if there are synonyms, get 1st
                    verb = verb.Substring(0, verb.IndexOf(","));
                }

                int verbId = verbMap[verb.ToUpper()];

                if (verbId != -1)
                {
                    string checkName = c.Attributes.GetNamedItem("check").Value;
                    checkList.Add(new Tuple<int,string>(verbId, checkName));
                }


            }

          
        }

        bool check_see_dobj()
        {
            return true; //legacy
        }

        bool check_have_dobj()
        {
            if (!VisibleAncestor(PLAYER,dobj))
            {
                PrintStringCr("YOU DON'T SEE THAT.");
                return false;
            }
            return true;
        }

        bool check_dont_have_dobj()
        {
            if (VisibleAncestor(PLAYER, dobj))
            {
                PrintStringCr("YOU ALREADY HAVE IT.");
                return false;
            }
            return true;
        }

        bool check_have_iobj()
        {
            if (!VisibleAncestor(PLAYER, iobj))
            {
                PrintStringCr("YOU DON'T HAVE THAT.");
                return false;
            }
            return true;
        }

        bool check_dobj_portable()
        {
            if (GetObjectAttr(dobj,"PORTABLE") == 0)
            {
                PrintStringCr("YOU CAN'T TAKE THAT.");
                return false;
            }
            return true;
        }


        bool check_light()
        {
            return PlayerCanSee();
        }

        bool check_dobj_supplied()
        {
            if (dobj == -1)
            {
                PrintStringCr("LOOKS LIKE YOU'RE MISSING A NOUN.");
                return false;
            }
            return true;
        }

        bool check_iobj_supplied()
        {
            if (iobj == -1)
            {
                PrintStringCr("LOOKS LIKE YOU'RE MISSING A NOUN.");
                return false;
            }
            return true;
        }


        bool check_prep_supplied()
        {
            if (prep == -1)
            {
                PrintStringCr("LOOKS LIKE YOU'RE MISSING A PREPOSITION?");
                return false;
            }
            return true;
        }

        bool check_dobj_unlocked()
        {
            if (GetObjectAttr(dobj, "LOCKED") == 1)
            {
                PrintStringCr("THE " + GetObjectName(dobj) + " IS LOCKED.");
                return false;
            }
            return true;
        }

        bool check_iobj_container()
        {
            if (GetObjectAttr(dobj, "CONTAINER") == 0)
            {
                PrintStringCr("THAT'S NOT A CONTAINER.");
                return false;
            }
            return true;
        }

        bool check_player_has_dobj()
        {
            if (!VisibleAncestor(PLAYER, dobj))
            {
                PrintStringCr("YOU DON'T SEE THAT.");
                return false;
            }
            return true;
        }

        bool check_dobj_closed()
        {
            if (GetObjectAttr(dobj, "OPEN") == 0)
            {
                return true;
            }
            PrintStringCr("IT'S ALREADY CLOSED.");
            return true;
        }


        bool check_dobj_opnable()
        {
            if (GetObjectAttr(dobj, "OPENABLE") == 0)
            {
                PrintStringCr("THAT'S NOT OPENABLE.");
                return false;
            }
            return true;
        }

        bool check_dobj_open()
        {
            throw new NotImplementedException();
           
        }

        bool check_not_self_or_child()
        {
            if (dobj == iobj || ObjectHas(dobj, iobj))
            {
                PrintStringCr("THAT'S NOT PHYSICALLY POSSIBLE.");
                return false;
            }
            return true;
        }

        bool RunChecks()
        {
                
                foreach (Tuple <int,string> t in checkList)
                {
                    if (t.Item1 == verb)
                    {
                        try {
                            VerbCheckDlgt chk = checkTable[t.Item2];
                            if (!chk())
                            {
                                return false;
                            }
                        } 
                        catch (Exception e)
                        {
                            throw new Exception("Unable to run check " + t.Item2 + " for verb " + t.Item1);     
                        }

                    }
                }
            

            return true;
        }
    }
}
