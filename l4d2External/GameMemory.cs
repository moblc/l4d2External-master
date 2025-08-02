using Swed32;
using System;
using l4d2External; // 用于Offsets类

namespace left4dead2Menu
{
    internal class GameMemory
    {
        public Swed swed { get; private set; }
        public Offsets offsets { get; private set; }
        public IntPtr client { get; private set; }
        public IntPtr engine { get; private set; }

        public GameMemory(string processName)
        {
            swed = new Swed(processName);
            offsets = new Offsets(); // 实例化偏移量
            InitializeModules();
        }

        private void InitializeModules()
        {
            client = swed.GetModuleBase("client.dll");
            engine = swed.GetModuleBase("engine.dll");

            // 用于初始调试的控制台消息
            if (client == IntPtr.Zero)
            {
                Console.WriteLine("Error CRÍTICO: No se pudo encontrar el módulo client.dll.");
            }
            else
            {
                Console.WriteLine($"Módulo client.dll cargado en: {client.ToString("X")}");
            }

            if (engine == IntPtr.Zero)
            {
                Console.WriteLine("Error CRÍTICO: No se pudo encontrar el módulo engine.dll.");
            }
            else
            {
                Console.WriteLine($"Módulo engine.dll cargado en: {engine.ToString("X")}");
            }
        }
    }
}