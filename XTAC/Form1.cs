﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using XMLtoAdv;

namespace XTAC
{
    public partial class Lantern : Form
    {
        public static Xml xproject;
        string fileName = "";

        string[] allChecks = new string[]
        {
            "check_see_dobj", "check_have_dobj", "check_dobj_supplied", "check_iobj_supplied","check_dont_have_dobj","check_not_self_or_child",
            "check_dobj_open", "check_dobj_closed", "check_dobj_opnable", "check_dobj_portable", "check_dobj_locked","check_dobj_unlocked",
            "check_iobj_container", "check_light"
        };

        static Lantern()
        {
            xproject = new Xml();
        }

        public Lantern()
        {
            InitializeComponent();

            //add fixed menu items
            sentenceTypeComboBox.Items.Add("before");
            sentenceTypeComboBox.Items.Add("instead");
            sentenceTypeComboBox.Items.Add("after");
            sentenceTypeComboBox.SelectedIndex = 1;

            foreach (String s in allChecks) {
                allChecksListBox.Items.Add(s);
            }/*
            allChecksListBox.Items.Add("check_see_dobj");
            allChecksListBox.Items.Add("check_have_dobj");
            allChecksListBox.Items.Add("check_dobj_supplied");
            allChecksListBox.Items.Add("check_iobj_supplied");
            allChecksListBox.Items.Add("check_dont_have_dobj");
            allChecksListBox.Items.Add("check_not_self_or_child");
            */
            NewProject();
            ShowProject();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Select an XML File";

            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a .CUR file was selected, open it.  
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Xml.Serialization.XmlSerializer reader = new XmlSerializer(typeof(Xml));
                userVerbListView.Clear();
                builtinVarsListBox.Items.Clear();
                // Read the XML file.
                StreamReader file = new StreamReader(openFileDialog1.FileName);
                fileName = openFileDialog1.FileName;

                // Deserialize the content of the file into a Book object.
                xproject = (Xml)reader.Deserialize(file);

                FixVariableNames();
                FixPrintedNames();
                FixFunctions();
                FixBlankDescriptions();

                ShowProject();
                file.Close();
                Text = "Lantern (" + fileName + ")";
            }
        }


        void ShowProject()
        {
            textBox4.Text = xproject.Project.ProjName;
            authorTextBox.Text = xproject.Project.Author;
            welcomeTextBox.Text = xproject.Project.Welcome;
            versionTextBox.Text = xproject.Project.Version;
            walkThroughTextBox.Text = xproject.Project.walkthrough;

            PopulateObjectDropDowns();
                
            if (objectsComboBox.Items.Count > 0)
            {
                objectsComboBox.SelectedIndex = 0;
            }

            LoadFunctions();
            PopulateVerbs();
            PopulateSentenceVerbs();
            PopulateEventRules();
            PopulateSentences();
            PopulatePrepositions();
            PopulateVars();

            sentenceIndirectObjComboBox.SelectedIndex = 0;
        }

        void PopulateObjectDropDowns()
        {
            PopulateListBox(objectsComboBox, false, false);
            PopulateListBox(nComboBox);
            PopulateListBox(sComboBox);
            PopulateListBox(eComboBox);
            PopulateListBox(wComboBox);
            PopulateListBox(neComboBox);
            PopulateListBox(nwComboBox);
            PopulateListBox(seComboBox);
            PopulateListBox(swComboBox);
            PopulateListBox(upComboBox);
            PopulateListBox(downComboBox);
            PopulateListBox(enterComboBox);
            PopulateListBox(outComboBox);
            PopulateListBox(parentComboBox, false);
            PopulateListBox(sentenceObjectComboBox, addEmpty: true, anyObject: true);
            PopulateListBox(sentenceIndirectObjComboBox, addEmpty: true, anyObject: true);

        }

        void PopulateSentenceVerbs()
        {
            verbComboBox.Items.Clear();
            foreach (String v in xproject.Project.Verbs.Builtinverbs.Verb)
            {
                verbComboBox.Items.Add(v);
            }
            foreach (String v in xproject.Project.Verbs.Userverbs.Verb)
            {
                verbComboBox.Items.Add(v);
            }

            verbComboBox.SelectedIndex = 0;
        }

        void LoadFunctions()
        {
            functionsListBox.Items.Clear();
            foreach (Routine r in xproject.Project.Routines.Routine)
            {
                functionsListBox.Items.Add(r.Name);
            }

            if (functionsListBox.Items.Count > 0)
            {
                functionsListBox.SelectedIndices.Add(0);
                codeTextBox.Text = xproject.Project.Routines.Routine[0].Text;
            }

            //load available functions into sentence drop down
            sentenceFuncComboBox.Items.Clear();
            foreach (Routine r in xproject.Project.Routines.Routine)
            {
                sentenceFuncComboBox.Items.Add(r.Name);
            }

            if (sentenceFuncComboBox.Items.Count > 0)   
                sentenceFuncComboBox.SelectedIndex = 0;

        }

        void PopulatePrepositions()
        {
            prepositionsComboBox.Items.Clear();
            prepositionsComboBox.Items.Add(""); //add blank for "no prep"
            foreach (String s in xproject.Project.Preps.Prep)
            {
                prepositionsComboBox.Items.Add(s);
            }
            prepositionsComboBox.SelectedIndex = 0;
        }

        void PopulateListBox(ComboBox cb, bool youCantGo = true, bool addEmpty=false, bool anyObject=false)
        {
            cb.Items.Clear();
            int i = 0;

            if (addEmpty) cb.Items.Add(""); //object lists can contain a blank

            foreach (var o in xproject.Project.Objects.Object)
            {
                cb.Items.Add(o.Name + "(" + i++ + ")");
            }

            if (youCantGo)  
                cb.Items.Add("You can't go that way.");
            
            if (anyObject)
                cb.Items.Add("*(ANY OBJECT)");

            if (cb.Items.Count > 0 && anyObject)
                cb.SelectedIndex = 0;
        }

        int GetIndex(string direction)
        {
            int index = Convert.ToInt32(direction);
            if (index == 255)
                return xproject.Project.Objects.Object.Count;
            else
                return index;
        }

        

        void PopulateSentences()
        {
            sentencesListBox.Items.Clear();

            foreach (Sentence s in xproject.Project.Sentences.Sentence)
            {
                sentencesListBox.Items.Add(BuildSentenceName(s));
            }
        }

        string BuildSentenceName(Sentence s)
        {
            string label = "("+ s.Type +") ";
                
            label += s.Verb;
            if (s.Do != "")
            {
                label += " " + s.Do;
            }
            if (s.Prep != "")
            {
                label += " " + s.Prep;
            }
            if (s.Io != "")
            {
                label += " " + s.Io;
            }

            label += "--->" + s.Sub;

            return label;
        }

        void PopulateEventRules()
        {
            rulesListBox.Items.Clear();
            foreach (Event r in xproject.Project.Events.Event)
            {
                rulesListBox.Items.Add(r.Name);
            }
        }

        void PopulateVerbs()
        {
            builtInVerbsTextBox.Text = "";

            foreach (string v in xproject.Project.Verbs.Builtinverbs.Verb)
            {

                builtInVerbsTextBox.Text += v + "\r\n";
                verbCheckListBox.Items.Add(v);
            }


            foreach (String s in xproject.Project.Verbs.Userverbs.Verb)
            {
                userVerbListView.Items.Add(s);
                verbCheckListBox.Items.Add(s);
            }
        }



        void SetRoomDirections()
        {
            int room = objectsComboBox.SelectedIndex;
            //nComboBox.SelectedIndex = -1;
            nComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.N);
            sComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.S);
            eComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.E);
            wComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.W);
            neComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Ne);
            nwComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Nw);
            seComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Se);
            swComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Sw);
            enterComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.In);
            outComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Out);
            upComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Up);
            downComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Down);
            parentComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Holder);
            massTextBox.Text = xproject.Project.Objects.Object[room].Directions.Mass;

            //`drinkableCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.);
        }

        bool GetCheck(string s)
        {
            if (s == null || s.Equals("0"))
            {
                return false;
            }
            else  
            {
                return true;
            }
        }

        void SetCheckBoxes()
        {
            int room = objectsComboBox.SelectedIndex;
            portableCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Portable);
            openCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Open);
            openableCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Openable);
            lockableCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Lockable);
            lockedCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Locked);
            emittingLightCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Emittinglight);
            visibleCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Scenery);
            backdropCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Backdrop);

            if (backdropCheckBox.Checked)
            {
                backdropTextBox.Text = xproject.Project.Objects.Object[room].Backdrop.Rooms;
                backdropTextBox.Enabled = true;
            }
            else
            {
                backdropTextBox.Text = "";
                backdropTextBox.Enabled = false;
            }

            doorCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Door);
            wearableCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Wearable);
            beingwornCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.BeingWorn);
            containerCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Container);
            supporterCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Supporter);
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "SW");
        }

        private void objectsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //update the connections and properties
            int i = objectsComboBox.SelectedIndex;

            Object o = xproject.Project.Objects.Object[i];

            objNameTextBox.Text = o.PrintedName;
            this.objectIdNameTextBox.Text = o.Name;
            objDescTextBox.Text = o.Description;
            initialDescTextBox.Text = o.Initialdescription;
            
            //set the holder
            int parent = Convert.ToInt32(o.Holder);
            parentComboBox.SelectedIndex = parent;

            SetRoomDirections();
            SetCheckBoxes();

            if (o.Synonyms != null)
            {
                synonymsTextBox.Text = o.Synonyms.Names;
            }
            else
            {
                o.Synonyms = new Synonyms();
                o.Synonyms.Names = "";
            }

            if (i < 2)
                objNameTextBox.ReadOnly = true;
            else
                objNameTextBox.ReadOnly = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void functionsListView_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("By Evan Wright, 2016-17");
        }

        private void movementAlertsButton_Click(object sender, EventArgs e)
        {
            if (GetCurObj() != null)
            {
                if (!GetCurObj().Id.Equals("1"))
                {
                    NoGoMessages ngm = new NoGoMessages(objectsComboBox.SelectedIndex);
                    ngm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Player doesn't have 'You can't go that way' messages.");
                }
            }
            else
            {
                MessageBox.Show("No object is selected.");
            }
        }

        private void newVerbButton_Click(object sender, EventArgs e)
        {
            //new verb button

            if (newVerbTextBox.Text.Equals(""))
            {
                MessageBox.Show("Verb can't be blank.");

            }
            else
            {
                //check for dupes!
                
                
                string v = newVerbTextBox.Text.Trim();

                if (!IsDupeVerb(v))
                {
                    xproject.Project.Verbs.Userverbs.Verb.Add(v);
                    userVerbListView.Items.Add(v);
                    verbComboBox.Items.Add(v);
                    verbCheckListBox.Items.Add(v);
                    newVerbTextBox.Text = "";
                }
                else
                {
                    MessageBox.Show("That verb already exists.");
                }
                
            }
        }

        private void newFuncButton_Click(object sender, EventArgs e)
        {
            //new verb button

            if (funcNameTextBox.Text.Equals(""))
            {
                MessageBox.Show("Function name can't be blank.");
            }
            else 
            {
                //check for duplicates
                string name = funcNameTextBox.Text;
                name = name.Replace(' ', '_');
                funcNameTextBox.Text = name;
                Routine r = new Routine();
                r.Name = name;
                
                r.Text = "";

                xproject.Project.Routines.Routine.Add(r);

                //update list boxes with functions
                functionsListBox.Items.Add(funcNameTextBox.Text);
                sentenceFuncComboBox.Items.Add(funcNameTextBox.Text);
                funcNameTextBox.Text = "";

                functionsListBox.SelectedIndices.Add(functionsListBox.Items.Count - 1);//select last
            }
        }

        private void addRuleButton_Click(object sender, EventArgs e)
        {
            //new verb button
            if (ruleNameTextBox.Text.Equals(""))
            {
                MessageBox.Show("Name can't be blank.");
            }
            else
            {
                string name = ruleNameTextBox.Text.Trim().Replace(' ', '_');
                //check for dupes!

                foreach (string s in this.rulesListBox.Items)
                {
                    if (s.ToUpper().Equals(name.ToUpper()))
                    {
                        MessageBox.Show("There is already an event with that name.");
                        ruleNameTextBox.Clear();
                        ruleNameTextBox.Focus();
                        return;
                    }
                }

                Event evt = new Event();
                
                ruleNameTextBox.Text = name;

                evt.Name = ruleNameTextBox.Text;
                evt.Text = "";
                xproject.Project.Events.Event.Add(evt);
                rulesListBox.Items.Add(evt.Name);
                rulesListBox.SelectedIndex = rulesListBox.Items.Count - 1;
                ruleCodeTextBox.Text = "";
                ruleNameTextBox.Text = "";
            }
        }

        private void ruleListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void ToggleProperty(Flags flags, string propName, bool newVal)
        {
            try
            {

                string newFlag = "1";
                if (newVal) newFlag = "0";

                flags.GetType().InvokeMember(propName,
                       BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                       Type.DefaultBinder, flags, new object[] { newFlag });

                

            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to set property " + propName);
            }
        }

        private void portableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; 
            ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        Object GetCurObj()
        {
            if (objectsComboBox.SelectedIndex == -1) { return null;  }
            return xproject.Project.Objects.Object[objectsComboBox.SelectedIndex];
        }

        private void containerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);

        }

        private void supporterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void visibleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);

            if (cb.Checked)
            { //scenery objects aren't portable
//                ToggleProperty(GetCurObj().Flags, "Portable", false);
                portableCheckBox.Checked = false;
            }
        }

        private void backdropCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);

            if (cb.Checked)
            {
//                ToggleProperty(GetCurObj().Flags, "Portable", false);
//                ToggleProperty(GetCurObj().Flags, "Scenery", true);
                portableCheckBox.Checked = false;
                visibleCheckBox.Checked = false;
                backdropTextBox.Enabled = true;
            }
            else
            {
                backdropTextBox.Enabled = false;
            }
        }

        private void openCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void openableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void lockableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void lockedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void emittingLightCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; 
            ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void doorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void edibleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void flammableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void drinkableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void rulesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rulesListBox.SelectedIndices.Count > 0)
            {//something selected
                
                int index = rulesListBox.SelectedIndices[0];
                string text = xproject.Project.Events.Event[index].Text;
                if (text != null && text.Length > 0)
                {
                    ruleCodeTextBox.Text = FormatCode(text);
                }
            }
        }

        private void functionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (functionsListBox.SelectedIndices.Count > 0)
            {
                codeTextBox.Text = FormatCode(xproject.Project.Routines.Routine[functionsListBox.SelectedIndices[0]].Text);
            }
        }


        string FormatCode(string code)
        {/*
            if (code != null)
            {
                string s = code;
                s = s.Replace("\r", "");
                s = s.Replace("\n", "");
                s = s.Replace("{", "\n{\n");
                s = s.Replace("}", "\n}\n");
                return s;
            }
            else
            {
                string s = "";
                return s;
            }*/
            return CodeFormatter.Format(code);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        Event FindEventRuleByName(string name)
        {
            foreach (Event e in xproject.Project.Events.Event)
            {
                if (e.Name == name)
                {
                    return e;
                }
            }

            return null;
        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void saveVarValueButton_Click(object sender, EventArgs e)
        {

            //add a new variable
            string varName = varNameTextBox.Text.Trim().Replace(' ', '_');
            varName = varName.Replace("$", string.Empty);
            //varNameTextBox.Text = varNameTextBox.Text.Trim().Replace(' ', '_');
            varNameTextBox.Text = varName;


            int val;
            if (!Int32.TryParse(valueTextBox.Text, out val))
            {
                MessageBox.Show("Variable value must be an integer.");
                valueTextBox.Focus();
                return;
            }

            if (val > 255)
            {
                MessageBox.Show("Variable value must be less than 256.  Yes, this is lame.");
                valueTextBox.Focus();
                return;
            }

            if (val < 0)
            {
                MessageBox.Show("Variable value must be between 0 and 256.  Yes, this is lame.");
                valueTextBox.Focus();
                return;
            }


            //check for duplicates in user and built-in variables
            IEnumerable<Var> vars = from v in xproject.Project.Variables.User.Var
                             where v.Name.Equals(varName)
                             select v;

            if (vars.Count<Var>() > 0)
            {
                vars.First<Var>().Value = val.ToString();
                MessageBox.Show("Variable updated.");
            }
            else
            {
                Var newVar = new Var();
                newVar.Name = varNameTextBox.Text;
                newVar.Value = val.ToString();
                newVar.Addr = Name.Replace(' ', '_');
                xproject.Project.Variables.User.Var.Add(newVar);
                userVarsListBox.Items.Add(newVar.Name);
            }

        }

        private void verbCheckListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //populate the checks based on what is already configured for that verb
            List<string> checks = new List<string>();

            foreach (string s in allChecks)
            {
                checks.Add(s);
            }

            string curVerb = verbCheckListBox.SelectedItem.ToString();
            List<string> verbChecks = new List<string>();

            if (curVerb.IndexOf(",") != -1)
            {
                curVerb = curVerb.Substring(0, curVerb.IndexOf(","));
            }

            var list =
                from c in xproject.Project.Checks.Check
                where c.Verb.Equals(curVerb)
                orderby c._check ascending
                select c;

            verbChecksListBox.Items.Clear();
            foreach (var v in list)
            {
                verbChecksListBox.Items.Add(v._check);
            }


        }

        void SetDirection(ComboBox cb, string dir)
        {
            string dirAttr = cb.SelectedIndex.ToString();

            if (cb.SelectedIndex == cb.Items.Count - 1)
            {//if last is selected, then "You can't go that way"
                dirAttr = "255";
            }

            if (dir.Equals("N"))
                GetCurObj().Directions.N = dirAttr;
            else if (dir.Equals("S"))
                GetCurObj().Directions.S = dirAttr;
            else if (dir.Equals("E"))
                GetCurObj().Directions.E = dirAttr;
            else if (dir.Equals("W"))
                GetCurObj().Directions.W = dirAttr;
            else if (dir.Equals("NE"))
                GetCurObj().Directions.Ne = dirAttr;
            else if (dir.Equals("SE"))
                GetCurObj().Directions.Se = dirAttr;
            else if (dir.Equals("SW"))
                GetCurObj().Directions.Sw = dirAttr;
            else if (dir.Equals("NW"))
                GetCurObj().Directions.Nw = dirAttr;
            else if (dir.Equals("UP"))
                GetCurObj().Directions.Up = dirAttr;
            else if (dir.Equals("DOWN"))
                GetCurObj().Directions.Down = dirAttr;
            else if (dir.Equals("IN"))
                GetCurObj().Directions.In = dirAttr;
            else if (dir.Equals("OUT"))
                GetCurObj().Directions.Out = dirAttr;
            else
                throw new Exception("Invalid direction " + dir);
        }

        private void nComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "N");
        }

        private void neComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "NE");
        }

        private void eComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "E");
        }

        private void nwComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "NW");
        }

        private void wComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "W");
        }

        private void outComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "OUT");
        }

        private void sComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "S");
        }

        private void upComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "UP");
        }

        private void downComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "DOWN");
        }

        private void parentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newSel = ((ComboBox)sender).SelectedIndex;
            int curObj = objectsComboBox.SelectedIndex;

            if (curObj != 0)
            {
                if (newSel != curObj)
                {
                    GetCurObj().Holder = ((ComboBox)sender).SelectedIndex.ToString();
                }
                else
                {
                    MessageBox.Show("An object cannot contain itself!");
                    ((ComboBox)sender).SelectedIndex = 0;
                }
            }
        }

        private void backdropTextBox_TextChanged(object sender, EventArgs e)
        {
            if (GetCurObj() != null)
            {
                GetCurObj().Backdrop.Rooms = backdropTextBox.Text;
            }
        }

        private void synonymsTextBox_TextChanged(object sender, EventArgs e)
        {
            GetCurObj().Synonyms.Names = ((TextBox)sender).Text.ToUpper();
        }

        void NewProject()
        {
            xproject = new Xml();
            xproject.Project = new Project();
            xproject.Project.ProjName = "NEW PROJECT";
            xproject.Project.Version = "VERSION 1.0";
            xproject.Project.Author = "YOUR NAME";
            xproject.Project.Welcome = "SHORT WELCOME MESSAGE.";
            xproject.Project.Objects = new Objects();
            xproject.Project.Objects.Object = new List<Object>();
            xproject.Project.Preps = new Preps();
            xproject.Project.Preps.Prep = new List<string>();
            xproject.Project.Variables = new Variables();
            xproject.Project.Variables.User = new User();
            xproject.Project.Variables.User.Var = new List<Var>();
            xproject.Project.Variables.Builtin = new Builtin();
            xproject.Project.Variables.Builtin.Var = new List<Var>();
            xproject.Project.Verbs = new Verbs();
            xproject.Project.Verbs.Builtinverbs = new Builtinverbs();
            xproject.Project.Verbs.Builtinverbs.Verb = new List<string>();
            xproject.Project.Verbs.Userverbs = new Userverbs();
            xproject.Project.Verbs.Userverbs.Verb = new List<string>();
            xproject.Project.Routines = new Routines();
            xproject.Project.Routines.Routine = new List<Routine>();
            xproject.Project.Checks = new Checks();
            xproject.Project.Checks.Check = new List<Check>();
            xproject.Project.Events = new Events();
            xproject.Project.Events.Event = new List<Event>();
            xproject.Project.Sentences = new Sentences();
            xproject.Project.Sentences.Sentence= new List<Sentence>();

            AddDefaultObjects();
            AddDefaultVerbs();
            AddDefaultVerbChecks();
            AddPrepositions();
            AddDefaultVars();
            AddDefaultFunctions();
            AddDefaultSentences();
            //ShowProject();
        }

        Object CreateObject(string name)
        {
            Object o = new Object();
            o.Name = name;
            o.PrintedName = name;
            o.Initialdescription = "";
            o.Id = xproject.Project.Objects.Object.Count.ToString();
            o.Description = "YOU NOTICE NOTHING UNEXPECTED.";

            o.Directions = new Directions();
            o.Directions.N = "255";
            o.Directions.S = "255";
            o.Directions.E = "255";
            o.Directions.W = "255";
            o.Directions.Ne = "255";
            o.Directions.Se = "255";
            o.Directions.Sw = "255";
            o.Directions.Nw = "255";
            o.Directions.Ne = "255";
            o.Directions.Down = "255";
            o.Directions.Up = "255";
            o.Directions.Out = "255";
            o.Directions.In = "255";
            o.Directions.Mass = "0";
            
            o.Holder = "0";
            
            o.Nogo = new Nogo();

            o.Synonyms = new Synonyms();
            o.Backdrop = new Backdrop();
            o.Flags = new Flags();

            o.Flags.Backdrop = "0";
            o.Flags.Portable = "0";
            o.Flags.Scenery = "0";
            o.Flags.Open = "0";
            o.Flags.Openable = "0";
            o.Flags.Locked = "0";
            o.Flags.Lockable = "0";
            o.Flags.Emittinglight = "0";
            o.Flags.Wearable = "0";
            o.Flags.Lightable = "0";
            o.Flags.Container = "0";
            o.Flags.Supporter = "0";
            o.Flags.Door = "0";
            o.Flags.BeingWorn = "0";
            o.Flags.Transparent = "0";
            o.Flags.Unused = "0";
            return o;
        }

        void AddDefaultObjects()
        {
            Object off = CreateObject("OFFSCREEN");
            off.Description = "OFFSTAGE.  MOVE OBJECTS HERE TO REMOVE THEM FROM THE WORLD.";
            xproject.Project.Objects.Object.Add(off);

            Object player = CreateObject("PLAYER");
            player.Holder = "2";
            player.Description = "YOU'RE A WONDERFUL PERSON. YOU SHOULDN'T CARE WHAT YOU LOOK LIKE.";
            player.Synonyms.Names = "ME,SELF";
            xproject.Project.Objects.Object.Add(player);

            Object room1 = CreateObject("ROOM 1");
            room1.Holder = "0";
            room1.Description = "THIS IS THE END OF A DIRT ROAD.";
            room1.Flags.Emittinglight="1";
            xproject.Project.Objects.Object.Add(room1);

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will delete your current project.  Proceed?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                userVerbListView.Clear();
                ruleCodeTextBox.Clear();
                NewProject();
                ShowProject();
            }
         }

        private void button1_Click(object sender, EventArgs e)
        {
            xproject.Project.Objects.Object.Add(CreateObject("OBJECT " + xproject.Project.Objects.Object.Count));
            
            //refresh list
            ShowProject();
            objectsComboBox.SelectedIndex = objectsComboBox.Items.Count - 1;
        }

        private void objNameTextBox_TextChanged(object sender, EventArgs e)
        {
            objNameTextBox.Text = objNameTextBox.Text.Trim().ToUpper();
            GetCurObj().PrintedName = objNameTextBox.Text;
        }

        private void objDescTextBox_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void initialDescTextBox_TextChanged(object sender, EventArgs e)
        {
            //GetCurObj().Initialdescription = initialDescTextBox.Text;
        }

       
        void CreateDefaultVar(string name, string addr, string val)
        {
            Var v = new Var();
            v.Name = name;
            v.Addr = addr;
            v.Value = val;
            xproject.Project.Variables.Builtin.Var.Add(v); //add to xml doc
            builtinVarsListBox.Items.Add(v.Name); //add to drop box
        }


        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //add sentence

            //make sure there isn't a sentence that matches that already
            Sentence s = new Sentence();
            //s.Verb = TrimVerb()
            s.Verb = TrimVerb(verbComboBox.SelectedItem.ToString());

            if (sentenceIndirectObjComboBox.SelectedItem.ToString() != "")
            {//make sure a do and a prep are supplied

                if (sentenceObjectComboBox.SelectedItem == null ||
                    sentenceObjectComboBox.SelectedItem.ToString().Equals(""))
                {
                    MessageBox.Show("If you have an indirect object, you also need a direct object and a preposition.");
                    return;
                }

            }


            s.Do = TrimNoun(sentenceObjectComboBox.SelectedItem.ToString());

            s.Prep = prepositionsComboBox.SelectedItem.ToString();
            s.Io = TrimNoun(sentenceIndirectObjComboBox.SelectedItem.ToString());

            if (this.sentenceFuncComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Function to call can't be blank.");
                return;
            }

            s.Sub = this.sentenceFuncComboBox.SelectedItem.ToString();
            s.Type = this.sentenceTypeComboBox.SelectedItem.ToString();
            if (!SentenceExists(s))
            {
                xproject.Project.Sentences.Sentence.Add(s); //add to xml
                PopulateSentences(); //refresh to list
            }
            else
            {
                MessageBox.Show("A sentence with those words already exists.");
            }
        }

        bool SentenceExists(Sentence s)
        {
            return false;
        }

        //removes trailing synonyms
        string TrimVerb(string verb)
        {
            if (verb.IndexOf(",")!=-1)
            {//has synonyms
                verb = verb.Substring(0, verb.IndexOf(",")).Trim();
            }
            return verb;
        }

        //removes the object number
        string TrimNoun(string name)
        {
            if (name.IndexOf('(') != -1)
            {
                name = name.Substring(0, name.IndexOf("(")).Trim();
            }
            return name;
        }


        private void builtinVarsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileName = SaveAs();
        }

        string SaveAs()
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog();
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Save as...";
         
            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a .CUR file was selected, open it.  
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                Save();
            }

            return fileName;      
        }

        private void authorTextBox_TextChanged(object sender, EventArgs e)
        {
            xproject.Project.Author = authorTextBox.Text;
        }

        private void sentencesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void codeTextBox_TextChanged(object sender, EventArgs e)
        {
            //get the current function
            string name = functionsListBox.SelectedItem.ToString();

            var funcs =
                    from f in xproject.Project.Routines.Routine
                    where f.Name.Equals(name)
                    select f;


            //update its code
            funcs.First<Routine>().Text = CodeFormatter.Format(codeTextBox.Text);
            

        }

        bool IsDupeVerb(string v)
        {
            foreach (ListViewItem lvi in userVerbListView.Items)
            {
                string[] toks = lvi.Text.Split(',');

                foreach (string item in toks)
                {
                    if (item.ToUpper().Equals(v.ToUpper()))
                    {
                        return true;
                    }
                }
                
            }


            /*
            foreach (ListViewItem lvi in this.builtInVerbsTextBox)
            {
                string[] toks = lvi.Text.Split(',');

                if (toks.Contains(v))
                    return true;

            }
            */
            char[] seps = { '\r', '\n', ',' };
            string builtIn = builtInVerbsTextBox.Text;
            string[] verbs = builtIn.Split(seps);

            foreach (string s in verbs)
            {
                foreach (string item in verbs)
                {
                    if (item.ToUpper().Equals(v.ToUpper()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void PopulateVars()
        {
            builtinVarsListBox.Items.Clear();
            foreach (var v in xproject.Project.Variables.Builtin.Var)
            {
                builtinVarsListBox.Items.Add(v.Name);
            }

            userVarsListBox.Items.Clear();
            foreach (var v in xproject.Project.Variables.User.Var)
            {
                userVarsListBox.Items.Add(v.Name);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //add a check to a verb
            //make sure it isn't already there first
            if (verbCheckListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a verb to add checks to.");
                return;
            }

            string verb = verbCheckListBox.SelectedItems[0].ToString();
            string check = allChecksListBox.SelectedItem.ToString();

            //is it a dupe?
            var checks = from c in xproject.Project.Checks.Check
                         where (c.Verb.Equals(verb) && c._check.Equals(check))
                         select c;

            if (checks.Count<Check>() > 0)
            {
                MessageBox.Show("That verb already has that check.");
                return;
            }

            Check ck = new Check();
            ck._check = check;
            ck.Verb = verb;
            xproject.Project.Checks.Check.Add(ck);

            verbChecksListBox.Items.Add(check);

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void versionTextBox_TextChanged(object sender, EventArgs e)
        {
            xproject.Project.Version = versionTextBox.Text.Trim().ToUpper();
        }

        private void welcomeTextBox_TextChanged(object sender, EventArgs e)
        {
            //
        }

        private void objNameTextBox_Leave(object sender, EventArgs e)
        {

            objNameTextBox.Text = objNameTextBox.Text.Trim().ToUpper();
            GetCurObj().Name = objNameTextBox.Text;
            int oldIndex = objectsComboBox.SelectedIndex;
            PopulateObjectDropDowns();
            //objectsComboBox. ; //refreshes text
            objectsComboBox.SelectedIndex = oldIndex;
        }
        
        private void initialDescTextBox_Leave(object sender, EventArgs e)
        {
            string temp = initialDescTextBox.Text.Trim().ToUpper();

            temp = temp.Replace('\"', '\'');
            temp = temp.Replace('\r', ' ');
            temp = temp.Replace('\n', ' ');
            initialDescTextBox.Text = temp;

            initialDescTextBox.Text = temp;
            GetCurObj().Initialdescription= temp;

        }

        private void objDescTextBox_Leave(object sender, EventArgs e)
        {
            if (objDescTextBox.Text.Length == 0)
            {
                objDescTextBox.Text = "YOU NOTICE NOTHING UNEXPECTED.";
            }
            else
            {
                string temp = objDescTextBox.Text.ToUpper().Trim();
                temp = temp.Replace('\"', '\'');
                temp = temp.Replace('\r', ' '); 
                temp = temp.Replace('\n', ' '); 
                GetCurObj().Description = temp;
                objDescTextBox.Text = temp;
            }
        }

        private void sentenceTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //GetCurSentence().Type=sentenceTypeComboBox.SelectedItem.ToString();
        }

        Sentence GetCurSentence()
        {
            return xproject.Project.Sentences.Sentence[sentenceTypeComboBox.SelectedIndex]; 
        }

        private void syntaxHelpButton_Click(object sender, EventArgs e)
        {
            SyntaxHelpForm sfh = new SyntaxHelpForm();
            sfh.Show();
        }

        private void enterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "IN");
        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void sentencesListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var index = sentencesListBox.IndexFromPoint(e.Location);

            if (index != ListBox.NoMatches)
            {
//                _selectedMenuItem = listBoxCollectionRounds.Items[index].ToString();
//                collectionRoundMenuStrip.Show(Cursor.Position);
//                collectionRoundMenuStrip.Visible = true;

                //context menu to confirm delete?
            }
             
        }

        private void sentencesListBox_KeyPress(object sender, KeyPressEventArgs e)
        {
         
        }

        private void sentencesListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                if (sentencesListBox.SelectedIndex != -1)
                {

                    int sel = sentencesListBox.SelectedIndex;
                    string s = sentencesListBox.Items[sel].ToString();

                    if (MessageBox.Show("Delete the sentence " + s + "?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                    
                        xproject.Project.Sentences.Sentence.RemoveAt(sel);
                        sentencesListBox.Items.RemoveAt(sel);

                        if (sel > 0)
                         sentencesListBox.SelectedIndex = sel-1;
                    }

                }
            }
        }

        private void ruleCodeTextBox_TextChanged(object sender, EventArgs e)
        {
            //save event
            try
            {
                string code = ruleCodeTextBox.Text;

                Event evnt = FindEventRuleByName(rulesListBox.SelectedItem.ToString());
                evnt.Text = code;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    msg += "\r\n" + ex.Message;
                }

                MessageBox.Show(msg);
            }
        }

        private void userVarsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                varNameTextBox.Text = xproject.Project.Variables.User.Var[userVarsListBox.SelectedIndex].Name;
                valueTextBox.Text = xproject.Project.Variables.User.Var[userVarsListBox.SelectedIndex].Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
             
            string name = textBox4.Text;
            name = name.Replace(' ','_');
            name = name.Replace(".","");
            xproject.Project.ProjName = name;
            textBox4.Text = name;
            
        }

        private void tRS80ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (fileName != "")
            {

                Save();

                try
                {
                    XmlToTables converter = XmlToTables.GetInstance();
                    converter.ConvertTRS80(fileName);  //"f3xml.xml"
                    MessageBox.Show("Export complete.  Open the directory " + converter.buildDir + " in Cygwin and run: build.sh");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException.Message);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
        }

        private void coCoToolStripMenuItem_Click(object sender, EventArgs e)
        {
//            fileName = Save();
            if (fileName != "")
            {
                Save();

                try
                {
                    XmlToTables converter = XmlToTables.GetInstance();
                    converter.Convert6809(fileName);  //"f3xml.xml"
                    MessageBox.Show("Export complete.  Open the directory " + converter.buildDir + " in Cygwin and run: build.sh");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException.Message);
                }
            }
            else
            {
                MessageBox.Show("File is null.");
            }

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void appleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {
                XmlToTables converter = XmlToTables.GetInstance();
                try
                {
                    Save();
                    converter.ConvertApple2(fileName);  //"f3xml.xml"
                    MessageBox.Show("Export complete.  Open the directory " + converter.buildDir + " in Cygwin and run: build.sh");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Export failed!" + ex.Message); 
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
        }

        private void functionsListBox_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                if (functionsListBox.SelectedIndex != -1)
                {

                    int sel = functionsListBox.SelectedIndex;
                    string s = functionsListBox.Items[sel].ToString();

                    if (MessageBox.Show("Delete the function " + s + "?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        xproject.Project.Routines.Routine.RemoveAt(sel);
                        functionsListBox.Items.RemoveAt(sel);

                        if (sel > 0)
                            sentencesListBox.SelectedIndex = sel - 1;
                    }

                }
            }
        }

        private void label54_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {//mass changed
            GetCurObj().Directions.Mass = massTextBox.Text.Trim();
        }

        private void label50_Click(object sender, EventArgs e)
        {

        }

        private void label49_Click(object sender, EventArgs e)
        {

        }

        private void zXSpectrumToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (fileName != "")
            {
                Save();
                XmlToTables converter = XmlToTables.GetInstance();
                try
                {
                    converter.ConvertSpectrum(fileName);  //"f3xml.xml"
                    MessageBox.Show("Export complete.  Open the directory " + converter.buildDir + " in Cygwin and run: build.sh");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Export failed!" + ex.Message + ex.InnerException.Message);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
        }

        private void commodore64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {
                Save();
                XmlToTables converter = XmlToTables.GetInstance();
                try
                {
                    converter.ConvertC64(fileName);  //"f3xml.xml"
                    MessageBox.Show("Export complete.  Open the directory " + converter.buildDir + " in Cygwin and run: build.sh");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Export failed!" + ex.Message + ex.InnerException.Message);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }

        }

        private void Save()
        {
            StreamWriter file = null;

            try 
            {
                System.Xml.Serialization.XmlSerializer writer = new XmlSerializer(typeof(Xml));
                // Write the XML file.
               
                file = new StreamWriter(fileName);
                // Serialize the project
                writer.Serialize(file, xproject);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        private void amstradCPC464ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {
                Save();
                try
                {
                    XmlToTables converter = XmlToTables.GetInstance();
                    converter.ConvertCPC464(fileName);  //"f3xml.xml"
                    MessageBox.Show("Export complete.  Open the directory " + converter.buildDir + " in Cygwin and run: build.sh or build.bat");
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    if (ex.InnerException != null)
                        msg += ex.InnerException.Message +"\r\n";

                    MessageBox.Show(msg);

                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            /*
            SyntaxChecker checker = new SyntaxChecker();

            try
            {
                checker.WriteRoutine(null, "test", codeTextBox.Text);
                MessageBox.Show("Looks good");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += ex.InnerException.Message;

                MessageBox.Show(msg);
            }
             */
            MessageBox.Show("Not implemented");
        }

        private void welcomeTextBox_Leave(object sender, EventArgs e)
        {
            xproject.Project.Welcome = welcomeTextBox.Text.Trim().ToUpper();
        }

        private void launchPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {
                   
               Save();

               if (ValidProj())
               {
                   //create a modeless dialog
                   TestClient tc = new TestClient();

                   try
                   {
                       tc.SetFile(fileName);
                       tc.ShowDialog();
                   }
                   catch (KeyNotFoundException knf)
                   {
                       MessageBox.Show("Unknown object or value: " + knf.Message);
                   }
                   catch (Exception ex)
                   {
                       
                       string msg = ex.Message;
                      
                       while (ex.InnerException != null)
                       {
                           ex = ex.InnerException;
                           msg += "\r\n" + ex.Message;
                       }

                       //MessageBox.Show(msg);
                       ExceptionForm ef = new ExceptionForm();
                       ef.ErrText = msg;
                   }
               }
            }
            else
            {
                SaveAs();
            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (fileName == "")
            {
                MessageBox.Show("There is no file loaded.");
            }
            else
            {
                Save();
            }
        }

        private void codeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\t')
            {
                codeTextBox.Paste("     ");
                e.Handled = true;
            }
        }

        private void codeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
             //   codeTextBox.Paste("   ");

                e.Handled = true;
            }
        }

        private void codeTextBox_TabStopChanged(object sender, EventArgs e)
        {
            int x = 5;
        }

        private void codeTextBox_TabIndexChanged(object sender, EventArgs e)
        {
            int x = 5;
        }

        private void codeTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true;
            }
        }

        private void ruleCodeTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true;
            }
        }

        private void ruleCodeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\t')
            {
                ruleCodeTextBox.Paste("     ");
                e.Handled = true;
            }
        }

        private void FromTrizbort()
        {
            string fileName = "";
            NewProject();
            TrizbortImporter ti = new TrizbortImporter(); //pass blank project to converter
            ti.Import(xproject, fileName);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Object obj = CreateObject("OBJECT " + xproject.Project.Objects.Object.Count);


            obj.Flags.Emittinglight = "1";

            xproject.Project.Objects.Object.Add(obj);

            //refresh list
            ShowProject();
            objectsComboBox.SelectedIndex = objectsComboBox.Items.Count - 1;
            
        }

        private void seComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "SE");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Object obj = CreateObject("OBJECT " + xproject.Project.Objects.Object.Count);

            obj.Flags.Door = "1";
            obj.Flags.Locked = "1";
            obj.Flags.Lockable = "1";
            obj.Flags.Open = "1";
            obj.Flags.Openable = "1";

            xproject.Project.Objects.Object.Add(obj);

            //refresh list
            ShowProject();
            objectsComboBox.SelectedIndex = objectsComboBox.Items.Count - 1;
        }

        private void importTrizbortFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will create a new project. Make sure you've save your project.\r\nDo you wish to continue?", "Confirm Import", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    openFileDialog1.Filter = "Trizbort Files|*.trizbort";
                    openFileDialog1.Title = "Select a Trizbort File";

                    // Show the Dialog.  
                    // If the user clicked OK in the dialog and  
                    // a .CUR file was selected, open it.  
                    if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {

                        NewProject();
                        TrizbortImporter trizImp = new TrizbortImporter();
                        trizImp.Import(xproject, openFileDialog1.FileName);
                        ShowProject();
                        MessageBox.Show("Import Complete. You will now be prompted to save your project.");
                        SaveAs();
//                        fileName = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void synonymsTextBox_Leave(object sender, EventArgs e)
        {

        }

        private void funcNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label70_Click(object sender, EventArgs e)
        {

        }

        private void ruleNameTextBox_TextChanged(object sender, EventArgs e)
        {
            //string ruleNameTextBox.Text;
        }

        private void label73_Click(object sender, EventArgs e)
        {

        }

        private void rulesListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (rulesListBox.SelectedIndex != -1)
            {
                rulesListBox.ContextMenuStrip = eventContextMenuStrip;
            }

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        //rename an event
        private void Rename_Click(object sender, EventArgs e)
        {
            RenameEventForm ren = new RenameEventForm();
           
            ren.OldName = rulesListBox.Items[rulesListBox.SelectedIndex].ToString();
            ren.ShowDialog();
            if (ren.result)
            {
                xproject.Project.Events.Event.ElementAt(rulesListBox.SelectedIndex).Name = ren.NewName;
                rulesListBox.Items[rulesListBox.SelectedIndex] = ren.NewName;
            }
        }

        //delete an event
        private void Delete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Really delete this event?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                rulesListBox.Items.RemoveAt(rulesListBox.SelectedIndex);
                ruleCodeTextBox.Text = "";
                rulesListBox.SelectedIndex = -1;
                rulesListBox.ContextMenuStrip = null;
            }

                
        }

        private void verbContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string verb = userVerbListView.Items[userVerbListView.SelectedIndices[0]].Text.ToString();
            //delete a verb
            //is the verb used in a sentence?
            foreach (Sentence s in xproject.Project.Sentences.Sentence)
            {
                if (s.Verb.Equals(verb))
                {
                    MessageBox.Show("Unable to delete verb.  It is being used by a sentence.");
                    return;
                }
            }

            //delete from project
            xproject.Project.Verbs.Userverbs.Verb.RemoveAt(userVerbListView.SelectedIndices[0]);  

            //delete from form
            userVerbListView.Items.RemoveAt(userVerbListView.SelectedIndices[0]);

            userVerbListView.ContextMenuStrip = null;

            //delete verb from verb check list
            for (int i=0; i < verbCheckListBox.Items.Count; i++)
            {
                if (verbCheckListBox.Items[i].ToString().Equals(verb))
                {
                    verbCheckListBox.Items.RemoveAt(i);
                    i--;
                }
            }

            //delete verb from rules
            
            for (int i = 0; i < xproject.Project.Checks.Check.Count; i++ )
            {
                if (xproject.Project.Checks.Check[i].Verb.Equals(verb))
                {
                    xproject.Project.Checks.Check.RemoveAt(i);
                    i--;
                }
            }
        }

        private void userVerbListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (userVerbListView.SelectedIndices.Count !=0)
            {
                userVerbListView.ContextMenuStrip = verbContextMenuStrip;
            }
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {

        }

        private void reviewTextBox_VisibleChanged(object sender, EventArgs e)
        {
            reviewTextBox.Clear();
            String s = "";
            foreach (Object o in xproject.Project.Objects.Object)
            {

                s += o.Name;
                try
                {
                    if (o.Flags.Portable.Equals("1")) { s += "(PORTABLE)"; }
                    if (o.Flags.Scenery.Equals("1")) { s += "(SCENERY)"; }
                    s += "\r\n";
                    s += "Description:" + o.Description + "\r\n";
                   
                    s += "Initial Description:" + o.Initialdescription + "\r\n";
                    s += "\r\n";
                }
                catch {
                    s += "\r\n";
                }
            }
            reviewTextBox.Text = s;
        }

        private void tabPage9_Click(object sender, EventArgs e)
        {

        }

        private bool ValidProj()
        {
            
            foreach (Object o in xproject.Project.Objects.Object)
            {
                try
                {
                    if (o.Flags.Door.Equals("1"))
                    {
                        int n = Convert.ToInt32(o.Directions.N);
                        int s = Convert.ToInt32(o.Directions.S);
                        int e = Convert.ToInt32(o.Directions.E);
                        int w = Convert.ToInt32(o.Directions.W);
                        int ne = Convert.ToInt32(o.Directions.Ne);
                        int se = Convert.ToInt32(o.Directions.Se);
                        int nw = Convert.ToInt32(o.Directions.Nw);
                        int sw = Convert.ToInt32(o.Directions.Sw);
                        int up = Convert.ToInt32(o.Directions.Up);
                        int dn = Convert.ToInt32(o.Directions.Down);

                        bool prob1 = (e == 255 && w != 255);
                        bool prob2 = (w == 255 && e != 255);

                        if ((n == 255 && s != 255) ||
                            (s == 255 && n != 255) ||
                            (e == 255 && w != 255) ||
                            (w == 255 && e != 255) ||
                            (nw == 255 && se != 255) ||
                            (se == 255 && nw != 255) ||
                            (sw == 255 && ne != 255) ||
                            (ne == 255 && sw != 255) ||
                            (up == 255 && dn != 255) ||
                            (dn == 255 && up != 255))
                        {
                            MessageBox.Show(o.Name + " is set as a door, but it is not connected to rooms on opposite sides.");
                            return false;
                        }
                    } 
                }
                catch (Exception ex)
                {

                }
            }//end foreach


            //warn about duplicate objects with the same name

            for (int i = 0; i < xproject.Project.Objects.Object.Count; i++)
            {
                Object o1 = xproject.Project.Objects.Object[i];
                for (int j = i+1; j < xproject.Project.Objects.Object.Count; j++)
                {
                    Object o2 = xproject.Project.Objects.Object[j];

                    if (o1.Name.ToUpper().Equals(o2.Name.ToUpper()))
                    { 
                        try
                        {
                            if (o1.Flags.Portable.Equals("1") && o2.Flags.Portable.Equals("1"))
                            {
                                MessageBox.Show("WARNING: You have two portable objects named:  " + o1.Name + "\r\nThis is NOT a good idea. ");
                                break;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
 
            return true;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void valueTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void valueTextBox_Leave(object sender, EventArgs e)
        {

        }

        private void codeTextBox_ParentChanged(object sender, EventArgs e)
        {

        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            xproject.Project.walkthrough = this.walkThroughTextBox.Text; 
        }
        
    }
}
