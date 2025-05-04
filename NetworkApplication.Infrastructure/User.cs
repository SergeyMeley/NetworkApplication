using NetworkApplication.ChatCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkApplication.Infrastructure
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ChatMessage> Messages { get; set; } // Связь с сообщениями
    }
}
