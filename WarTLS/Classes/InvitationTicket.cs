using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WARTLS.CLASSES;

namespace WARTLS.CLASSES
{
    class InvitationTicket
    {
        internal string ID;
        internal Client Sender;
        internal Client Receiver;
        internal string GroupId;
        internal bool IsFollow = false;
        internal byte Result = 255;
        internal InvitationTicket(Client Sender,Client Receiver)
        {
            this.ID = Sender.Player.Random.Next(1, Int32.MaxValue).ToString();
            this.Sender = Sender;
            this.Receiver = Receiver;
        }
    }
}
