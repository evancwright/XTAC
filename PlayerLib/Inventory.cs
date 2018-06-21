using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerLib
{
    partial class Game
    {
     

        public void Inventory()
        {
            if (HasVisibleItems(PLAYER))
            {
                PrintStringCr("YOU ARE CARRYING...");
                ListContents(PLAYER, true);
            }
            else
            {
                PrintStringCr("YOU ARE EMPTY HANDED.");
            }

        }



        void ListContents(int obj, bool printAdjs = false)
        {
            for (int i = 2; i < objTable.GetCount(); i++ )
            {
                if (objTable.GetObjAttr(i, "HOLDER") == obj)
                {
                    if (objTable.GetObjAttr(i, "SCENERY")==0)
                    {

                        PrintObjectName(i);
                        if (printAdjs)
                        {
                            if (GetObjectAttr(i,"LIT")==1)
                                PrintString("(PROVIDING LIGHT)");
                            if (GetObjectAttr(i, "BEINGWORN") == 1)
                                PrintString("(BEING WORN)");

                        }
                        PrintCr();

                        if (ShouldListContents(i))
                        {
                            if (GetObjectAttr(i, "SUPPORTER") == 1)
                            {
                                PrintString("ON THE ");
                                PrintObjectName(i);
                                PrintStringCr(" IS...");
                            }
                            else
                            {
                                PrintString("THE ");
                                PrintObjectName(i);
                                PrintStringCr(" CONTIAINS...");
                            }
                            ListContents(i,printAdjs);
                        }
                    }
                }
            }
        }


        void ListRoomObjects()
        {
            int currentRoom = GetPlayerRoom();

            for (int i = 2; i < objTable.GetCount(); i++)
            {
                if (objTable.GetObjAttr(i, "HOLDER") == currentRoom)
                {
                    if (objTable.GetObjAttr(i, "SCENERY") == 0)
                    {

                        int strId = objTable.GetObjAttr(i, "INITIALDESCRIPTION");
                            

                        if (strId == -1 || strId == 255 || stringTable.GetEntry(strId) == "")
                        {
                            PrintStringCr("THERE IS A " + objTable.objects[i].name + " HERE");
                        }
                        else
                        {
                            PrintStringCr(objTable.GetObjAttr(i,"INITIALDESCRIPTION"));
                        }

                        if (ShouldListContents(i))
                        {
                            if (GetObjectAttr(i,"SUPPORTER")==1)
                            {
                                PrintString("ON THE ");
                                PrintObjectName(i);
                                PrintStringCr(" IS...");
                            }
                            else
                            { 
                                PrintString("THE ");
                                PrintObjectName(i);
                                PrintStringCr(" CONTIAINS...");
                            }
                            ListContents(i);
                        }
                    }
                }
            }
        
        }

        bool ShouldListContents(int i)
        {
            if (objTable.GetObjAttr(i, "SUPPORTER")==1 ||
                (objTable.GetObjAttr(i, "CONTAINER")==1 && objTable.GetObjAttr(i, "OPEN")==1)
                )
            {
                if (HasVisibleItems(i))
                    return true;
            }
            return false;
        }

        void Indent()
        {
        }

        bool VisibleAncestor(int obj1, int obj2)
        {
        
            while (true)
            {
                int parent = GetObjectAttr(obj2, "HOLDER");

                if (parent == obj1)
                    return true;

                if (parent == OFFSCREEN)
                   return false;

                if (IsClosedContainer(parent))
                    return false;

                obj2 = parent;
            }

        }

    }
}
