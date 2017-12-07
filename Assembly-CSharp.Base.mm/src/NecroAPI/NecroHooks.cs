using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbraxisToolset {
    public static class NecroHooks {

        /// <summary>
        /// This hook is called whenever a level finishes loading.
        /// </summary>
        public static NecroHookFunction onCellsFinished = delegate (object[] args) { };
        public static NecroHookFunction onCellCreated = delegate (object[] args) { };

        public delegate void NecroHookFunction(params object[] args);
    }
}
