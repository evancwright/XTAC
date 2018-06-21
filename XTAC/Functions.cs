using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XTAC
{
    public partial class Lantern : Form
    {

        void AddDefaultFunctions()
        {

            Routine r = new Routine();

            r.Name = "not_possible";
            r.Text = "if (dobj == player) { println(\"NOT PHYSICALLY POSSIBLE.\");  } ";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "get_portable";
            r.Text = "if (dobj.portable == 1) { if (dobj.holder != player) { println(\"(TAKEN)\"); dobj.holder = player;}  } ";
            xproject.Project.Routines.Routine.Add(r);

            //kill self routine
            r = new Routine();
            r.Name = "kill_self";
            r.Text = "println(\"IF YOU ARE EXPERIENCING SUICIDAL THOUGHTS, YOU SHOULD SEEK PSYCHIATRIC HELP.\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "default_kill";
            r.Text = "println(\"PERHAPS YOU SHOULD COUNT TO 3 AND CALM DOWN.\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "kill_player";
            r.Text = "println(\"***YOU HAVE DIED***.\");player.holder=2;\n";
            xproject.Project.Routines.Routine.Add(r);

            //kill self routine
            r = new Routine();
            r.Name = "talk_to_self";
            r.Text = "println(\"TALKING TO YOURSELF IS A SIGN OF IMPENDING MENTAL COLLAPSE.\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "default_talk";
            r.Text = "println(\"THAT DOES PRODUCE AN EXCITING CONVERSATION.\");";
            xproject.Project.Routines.Routine.Add(r);

            //listen
            r = new Routine();
            r.Name = "listen";
            r.Text = "println(\"YOU HEAR NOTHING UNEXPECTED.\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "smell";
            r.Text = "println(\"YOU SMELL NOTHING UNEXPECTED.\");";
            xproject.Project.Routines.Routine.Add(r);


            r = new Routine();
            r.Name = "wait";
            r.Text = "println(\"TIME PASSES...\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "yell";
            r.Text = "println(\"AAAAAAAAAAAAARRRRGGGGGG!\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "jump";
            r.Text = "println(\"WHEEEEEE!\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "default_eat";
            r.Text = "println(\"THAT'S NOT PART OF A HEALTHY DIET.\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "default_drink";
            r.Text = "println(\"THAT'S HARDLY A REFRESHING DRINK.\");";
            xproject.Project.Routines.Routine.Add(r);
        }

        void AddDefaultVars()
        {
            //add variables to a newly created project
            builtinVarsListBox.Items.Clear();
            CreateDefaultVar("dobj", "sentence+1", "0");
            CreateDefaultVar("iobj", "sentence+3", "0");
            CreateDefaultVar("score", "score", "0");
            CreateDefaultVar("moves", "moves", "0");
            CreateDefaultVar("health", "health", "100");
            CreateDefaultVar("turnsWithoutLight", "turnsWithoutLight", "0");
            CreateDefaultVar("gameOver", "gameOver", "0");
            

        }


        void AddDefaultSentences()
        {
            Sentence s = new Sentence();
            s = new Sentence();
            s.Verb = "examine";
            s.Do = "*";
            s.Io = "";
            s.Prep = "";
            s.Sub = "get_portable";
            s.Type = "before";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "take";
            s.Do = "PLAYER";
            s.Io = "";
            s.Prep = "";
            s.Sub = "not_possible";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "kill";
            s.Do = "PLAYER";
            s.Io = "";
            s.Prep = "";
            s.Sub = "kill_self";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "kill";
            s.Do = "*";
            s.Io = "";
            s.Prep = "";
            s.Sub = "default_kill";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "kill";
            s.Do = "*";
            s.Io = "*";
            s.Prep = "with";
            s.Sub = "default_kill";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "talk to";
            s.Do = "PLAYER";
            s.Io = "";
            s.Prep = "";
            s.Sub = "talk_to_self";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "talk to";
            s.Do = "*";
            s.Io = "";
            s.Prep = "";
            s.Sub = "default_talk";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "listen";
            s.Do = "";
            s.Io = "";
            s.Prep = "";
            s.Sub = "listen";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "smell";
            s.Do = "";
            s.Io = "";
            s.Prep = "";
            s.Sub = "smell";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);


            s = new Sentence();
            s.Verb = "wait";
            s.Do = "";
            s.Io = "";
            s.Prep = "";
            s.Sub = "wait";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);


            s = new Sentence();
            s.Verb = "yell";
            s.Do = "";
            s.Io = "";
            s.Prep = "";
            s.Sub = "yell";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "jump";
            s.Do = "";
            s.Io = "";
            s.Prep = "";
            s.Sub = "jump";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "eat";
            s.Do = "*";
            s.Io = "";
            s.Prep = "";
            s.Sub = "default_eat";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);


            s = new Sentence();
            s.Verb = "drink";
            s.Do = "*";
            s.Io = "";
            s.Prep = "";
            s.Sub = "default_drink";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "wear";
            s.Do = "*";
            s.Io = "";
            s.Prep = "";
            s.Sub = "get_portable";
            s.Type = "before";
            xproject.Project.Sentences.Sentence.Add(s);

        }


        void AddDefaultVerbChecks()
        {
            AddCheck("close", "check_dobj_supplied");
            AddCheck("drink", "check_dobj_supplied");
            AddCheck("drink", "check_have_dobj");
            AddCheck("drop", "check_dobj_supplied");
            AddCheck("drop", "check_have_dobj");
            AddCheck("eat", "check_dobj_supplied");
            AddCheck("enter", "check_dobj_supplied");
            AddCheck("examine", "check_dobj_supplied");
            AddCheck("get", "check_dobj_supplied");
            AddCheck("get", "check_dont_have_dobj");
            AddCheck("get", "check_dobj_portable");
            AddCheck("kill", "check_dobj_supplied");
            AddCheck("light", "check_dobj_supplied");
            AddCheck("light", "check_have_dobj");
            AddCheck("open", "check_dobj_supplied");
            AddCheck("open", "check_dobj_opnable");
            AddCheck("open", "check_dobj_unlocked");
            AddCheck("put", "check_dobj_supplied");
            AddCheck("put", "check_prep_supplied");
            AddCheck("put", "check_iobj_supplied");
            AddCheck("put", "check_not_self_or_child");
            AddCheck("talk to", "check_dobj_supplied");
            AddCheck("turn on", "check_dobj_supplied");
            //AddCheck("turn on", "check_have_dobj");
            AddCheck("unlock", "check_dobj_supplied");
            AddCheck("look in", "check_dobj_supplied");
        }

        void AddCheck(string verb, string check)
        {
            Check c = new Check();
            c.Verb = verb;
            c._check = check;
            xproject.Project.Checks.Check.Add(c);
        }

        void AddPrepositions()
        {
            string[] defaultPreps = new string[] { "in", "on", "at", "under", "into", "inside", "through", "out", "behind", "off", "up", "with", "to", "off" };

            xproject.Project.Preps.Prep = new List<string>();
            foreach (string s in defaultPreps) { xproject.Project.Preps.Prep.Add(s); }
        }

        void AddDefaultVerbs()
        {

            string[] defaultVerbs = new string[] { 
                "n,go north,north","s,go south,south","e,go east,east","w,go west,west","ne,go northeast,northeast","se,go southeast,southeast","sw,go southwest,southwest","nw,go northwest,northwest",
"up,go up,u","down,go down,d","enter,go in,go into,go inside","out","get,take,grab,pick up","give","inventory,i","kill","drop","light","look,l","examine,x,look at,inspect","look in,search","open","lock","unlock","close,shut","eat","drink","put,place","quit","smell,sniff","listen","wait","climb",
"yell,scream,shout", "jump", "talk to", "turn on","wear", "turn off", "save", "restore"
            };

            foreach (string s in defaultVerbs) { xproject.Project.Verbs.Builtinverbs.Verb.Add(s); }
        }


        void FixVariableNames()
        {
            foreach (Var v in xproject.Project.Variables.User.Var)
            {
                if (v.Name[0] == '$')
                {
                    v.Name = v.Name.Substring(1); //chop off $
                }
            }
        }


        void FixPrintedNames()
        {
            foreach (Object o in xproject.Project.Objects.Object)
            {
                if (o.PrintedName == null || o.PrintedName == "")
                {
                    //if the printed name is null, switch the name and the printed name
                    o.PrintedName = o.Name.Trim();
                    o.Name = o.Name.Replace(' ', '_');
                }

                if (o.PrintedName.Contains('_'))
                {
                    o.PrintedName = o.PrintedName.Replace('_', ' ');
                }
            }

        }

        void FixBlankDescriptions()
        {
            foreach (Object o in xproject.Project.Objects.Object)
            {
                if (o.Description == null || o.Description == "")
                {
                    //if the printed name is null, switch the name and the printed name
                    o.Description = "YOU NOTICE NOTHING UNEXPECTED.";
                }
            }

        }

        void FixFunctions()
        {
            foreach (Routine r in xproject.Project.Routines.Routine )
            {
                r.Name = r.Name.Trim().Replace(' ', '_');
            }

            foreach (Event e in xproject.Project.Events.Event)
            {
                e.Name = e.Name.Trim().Replace(' ', '_');
            }

            foreach (Sentence s in xproject.Project.Sentences.Sentence)
            {
                s.Sub = s.Sub.Trim().Replace(' ', '_');
            }


            

        }


    }
}
