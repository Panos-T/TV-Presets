using System;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.Diagnostics;		    	// For System Monitor Access
using Crestron.SimplSharpPro.DeviceSupport;         	// For Generic Device Support

using Crestron.SimplSharp.CrestronIO;                   //For File Access
using Crestron.SimplSharpPro.GeneralIO;                 //For IR Control
using Crestron.SimplSharpPro.EthernetCommunication;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;


namespace TV_Presets
{
    public class ControlSystem : CrestronControlSystem
    {
        // Hardware containers

        private IROutputPort presetsPort;
        private EthernetIntersystemCommunications myEISC;
      //  private Task myTask;

        
        RootObject ro = new RootObject();
        string JSONDataAsAString = "";
        string filepath = "/NVRAM/localization.json";

        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                CrestronConsole.AddNewConsoleCommand(ReadJSON, "ReadJSON", "Read JSON File.", ConsoleAccessLevelEnum.AccessAdministrator);
                CrestronConsole.AddNewConsoleCommand(CallChannelByName, "CallChannel", "Call Channel by Name", ConsoleAccessLevelEnum.AccessAdministrator);


                //Subscribe to the controller events (System, Program, and Ethernet)
                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(_ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(_ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(_ControllerEthernetEventHandler);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        //-----------------> Custom Functions

        public void CallCMD(int cmdNum)
        {
            switch (cmdNum)
            {
                case (0):
                    presetsPort.PressAndRelease("0", 200);
                    CrestronConsole.PrintLine("Calling 0");
                    break;
                case (1):
                    presetsPort.PressAndRelease("1", 200);
                    CrestronConsole.PrintLine("Calling 1");
                    break;
                case (2):
                    presetsPort.PressAndRelease("2", 200);
                    CrestronConsole.PrintLine("Calling 2");
                    break;
                case (3):
                    presetsPort.PressAndRelease("3", 200);
                    CrestronConsole.PrintLine("Calling 3");
                    break;
                case (4):
                    presetsPort.PressAndRelease("4", 200);
                    CrestronConsole.PrintLine("Calling 4");
                    break;
                case (5):
                    presetsPort.PressAndRelease("5", 200);
                    CrestronConsole.PrintLine("Calling 5");
                    break;
                case (6):
                    presetsPort.PressAndRelease("6", 200);
                    CrestronConsole.PrintLine("Calling 6");
                    break;
                case (7):
                    presetsPort.PressAndRelease("7", 200);
                    CrestronConsole.PrintLine("Calling 7");
                    break;
                case (8):
                    presetsPort.PressAndRelease("8", 200);
                    CrestronConsole.PrintLine("Calling 8");
                    break;
                case (9):
                    presetsPort.PressAndRelease("9", 200);
                    CrestronConsole.PrintLine("Calling 9");
                    break;
            }
        }


        public void SetChannel(ushort channelNum)
        {

            int firstD, secondD, thirdD;

            firstD = channelNum / 100;
            secondD = channelNum / 10 % 10;
            thirdD = channelNum % 10;

            // CrestronConsole.PrintLine("Calling Channel {0} - {1} - {2}.", firstD, secondD, thirdD);
            Task.Run( async () =>
            {
                CallCMD(firstD);
                await Task.Delay(300);
                CallCMD(secondD);
                await Task.Delay(300);
                CallCMD(thirdD);

            });
            

        }





        public void CallChannelByName(string args)
        {
            // string[] arrayArgs = args.Split(' ');
            string channelToCall = args; //arrayArgs[0];
            ushort chIndex = 0;


            CrestronConsole.PrintLine("Trying to Call {0}", channelToCall);


            // CrestronConsole.PrintLine("Searching for channel Index!");
            chIndex = ReturnNum(channelToCall);
            //CrestronConsole.PrintLine("Channel Exists with Index {0}", chIndex);
            
            

            CrestronConsole.PrintLine("ChNum {0}", chIndex);

            if (chIndex > 0)
            {
                CrestronConsole.PrintLine("Calling IR Func");
                
                SetChannel(chIndex);
            }
            else
            {
                CrestronConsole.PrintLine("There is no Channel Named: {0}", channelToCall);
            }

        }

        public ushort ReturnNum(string chParam)
        {

            CrestronConsole.PrintLine("Searching for Param");
            foreach (ChannelName element in ro.NOVACHANNELS)
            {
                if (element.chName == chParam)
                {
                    return element.chNum;
                }
                CrestronConsole.PrintLine("End Of Search");
               
            }
            return 0;
        }



        //----------------------------------> Read JSON

        void ReadJSON(string args)
        {


          //  int chIndex = -1;
          //  uint chVar = 0;
         //   ChannelName chtest = new ChannelName();
          //  chtest.chName = "Channel 1";
            try
            {
                using (StreamReader sr = new StreamReader(filepath, System.Text.Encoding.Default))
                    JSONDataAsAString = sr.ReadToEnd();
               
                
                CrestronConsole.PrintLine("JSON Data: {0}", JSONDataAsAString);


                ro = JsonConvert.DeserializeObject<RootObject>(JSONDataAsAString);

               /* 
               

                CrestronConsole.PrintLine("Number of fields: {0}", ro.Channels.Count);
                //  CrestronConsole.PrintLine("Index: {0}", ro.Channels.IndexOf(chtest));

                chIndex = ro.Channels.FindIndex(p => p.chName == "Channel 1");
                chVar = ro.Channels[chIndex].chNum;
                
                
                CrestronConsole.PrintLine("Index: {0}", chIndex);
                CrestronConsole.PrintLine("chVar: {0}.", chVar);

                CrestronConsole.PrintLine("VarType-> {0} | JSONType-> {1}",chVar.GetType(), ro.Channels[chIndex].chName.GetType());

                CrestronConsole.PrintLine("Trying to print Directly name: {0}", ro.Channels[chIndex].chName.Normalize());
                CrestronConsole.PrintLine("Trying to print Directly index as uint: {0}", ro.Channels[chIndex].chNum);
               */
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("JSON Error: {0}: ", e.Message);
            }
        }




        public class ChannelName
        {
            [JsonProperty("No.")]
            public string No { get; set; }

            [JsonProperty("CHANNEL NAME")]
            public string chName { get; set; }

            [JsonProperty("FREQUENCY No.")]
            public ushort chNum { get; set; }
        }

        public class RootObject
        {
            [JsonProperty("NOVA CHANNELS")]
            public List<ChannelName> NOVACHANNELS { get; set; }
        }

        /*


        public class RootObject
        {
            public List<ChannelName> Channels { get; set; }
        }

        public class ChannelName
        {
            public string chName { get; set; }
            public ushort chNum { get; set; }
        }

        */





        /// <summary>
        /// InitializeSystem - this method gets called after the constructor 
        /// has finished. 
        /// 
        /// Use InitializeSystem to:
        /// * Start threads
        /// * Configure ports, such as serial and verisports
        /// * Start and initialize socket connections
        /// Send initial device configurations
        /// 
        /// Please be aware that InitializeSystem needs to exit quickly also; 
        /// if it doesn't exit in time, the SIMPL#Pro program will exit.
        /// </summary>
        public override void InitializeSystem()
        {
            try
            {


                //IR Lirary Path
                string IRLibPath = string.Format("{0}/SkyQ.ir", Directory.GetApplicationDirectory());
                CrestronConsole.PrintLine(">>>>> IRLibPath: {0}", IRLibPath);


                if (this.SupportsIROut)
                {
                    if (ControllerIROutputSlot.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                    {
                        CrestronConsole.PrintLine("Could not Register IR Output reason: {0}", ControllerIROutputSlot.DeviceUnRegistrationFailureReason);
                    }
                    else
                    {
                        presetsPort = IROutputPorts[1];

                    }
                }

                presetsPort.LoadIRDriver(IRLibPath);


                myEISC = new EthernetIntersystemCommunications(0xF0, "127.0.0.2", this);
                myEISC.SigChange += MyEISC_SigChange;
                myEISC.OnlineStatusChange += MyEISC_OnlineStatusChange;

                myEISC.Register();

            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        private void MyEISC_OnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            if (args.DeviceOnLine)
            {
                CrestronConsole.PrintLine("EISC Online");
            }
            else
            {
                CrestronConsole.PrintLine("EISC Offline");
            }
        }

        private void MyEISC_SigChange(BasicTriList currentDevice, SigEventArgs args)
        {



            CrestronConsole.PrintLine("Signal Change Event");
            if (args.Sig.Type.Equals(eSigType.UShort))
            {

                CrestronConsole.PrintLine("Analog Sig Changed");
                if (args.Sig.Number == 1)
                {
                    CrestronConsole.PrintLine("Calling for TV Preset num {0}!", args.Sig.UShortValue);
                    SetChannel(args.Sig.UShortValue);
                }
            }

            if (args.Sig.Type.Equals(eSigType.String))
            {
                CrestronConsole.PrintLine("Serial Sig Changed");
                if (args.Sig.Number == 1)
                {
                    CrestronConsole.PrintLine("Asking for TV Preset named {0}", args.Sig.StringValue);
                    CallChannelByName(args.Sig.StringValue);
                }
            }
            
        }

        /// <summary>
        /// Event Handler for Ethernet events: Link Up and Link Down. 
        /// Use these events to close / re-open sockets, etc. 
        /// </summary>
        /// <param name="ethernetEventArgs">This parameter holds the values 
        /// such as whether it's a Link Up or Link Down event. It will also indicate 
        /// wich Ethernet adapter this event belongs to.
        /// </param>
        void _ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {//Determine the event type Link Up or Link Down
                case (eEthernetEventType.LinkDown):
                    //Next need to determine which adapter the event is for. 
                    //LAN is the adapter is the port connected to external networks.
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        //
                    }
                    break;
                case (eEthernetEventType.LinkUp):
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// Event Handler for Programmatic events: Stop, Pause, Resume.
        /// Use this event to clean up when a program is stopping, pausing, and resuming.
        /// This event only applies to this SIMPL#Pro program, it doesn't receive events
        /// for other programs stopping
        /// </summary>
        /// <param name="programStatusEventType"></param>
        void _ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case (eProgramStatusEventType.Paused):
                    //The program has been paused.  Pause all user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Resumed):
                    //The program has been resumed. Resume all the user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Stopping):
                    //The program has been stopped.
                    //Close all threads. 
                    //Shutdown all Client/Servers in the system.
                    //General cleanup.
                    //Unsubscribe to all System Monitor events
                    break;
            }

        }

        /// <summary>
        /// Event Handler for system events, Disk Inserted/Ejected, and Reboot
        /// Use this event to clean up when someone types in reboot, or when your SD /USB
        /// removable media is ejected / re-inserted.
        /// </summary>
        /// <param name="systemEventType"></param>
        void _ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case (eSystemEventType.DiskInserted):
                    //Removable media was detected on the system
                    break;
                case (eSystemEventType.DiskRemoved):
                    //Removable media was detached from the system
                    break;
                case (eSystemEventType.Rebooting):
                    //The system is rebooting. 
                    //Very limited time to preform clean up and save any settings to disk.
                    break;
            }

        }
    }
}