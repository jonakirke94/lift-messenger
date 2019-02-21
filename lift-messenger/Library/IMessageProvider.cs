using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace lift_messenger.Library
{
    public interface IMessageProvider
    {
        Task SendMessage(string message);

        string GenerateMessage();
    }
}
