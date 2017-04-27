using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaboodleES;
using OpenTK;
using OpenTK.Input;

namespace LightCyclesAI.Events
{
    public class KeyboardKeyEvent : IEvent
    {
        public KeyboardKeyEventArgs keyboardKeyEvent;

        public KeyboardKeyEvent(KeyboardKeyEventArgs e)
        {
            this.keyboardKeyEvent = e;
        }
    }
}
