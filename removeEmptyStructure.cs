using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
[assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
    public class Script
    {
        const string SCRIPT_NAME = "removeEmptyStructure";
        public Script()
        {
        }

        //[MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context /*, System.Windows.Window window, ScriptEnvironment environment*/)
        {
            // TODO : Add here the code that is called when the script is launched from Eclipse.
            if (context.Patient == null || context.StructureSet == null)
            {
                MessageBox.Show("Please load a patient, 3D image, and structure set before running this script.", SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            context.Patient.BeginModifications();   // enable writing with this script.

            StructureSet structureSet = context.StructureSet;

            string log = "";
            var removedStructureIds = new List<string> { };
            var unremovedStructureIds = new List<string> { };

            //search empty structure.
            foreach (var structure in structureSet.Structures)
            {
                if (structure.IsEmpty == true)
                {
                    if (structureSet.CanRemoveStructure(structure) == true)
                    {
                        removedStructureIds.Add(structure.Id);
                        log += structure.Id + " : removed" + "\n";
                    }
                    else
                    {
                        unremovedStructureIds.Add(structure.Id);
                        log += structure.Id + " : Not removed(chage approval status!)" + "\n";
                    }
                }
            }

            //remove empty structure.
            foreach (var id in removedStructureIds)
            {
                if (structureSet.Structures.Any(st => st.Id == id))
                {
                    var removedStructure = structureSet.Structures.Single(st => st.Id == id);
                    structureSet.RemoveStructure(removedStructure);
                }
            }

            //show complete message.
            MessageBox.Show(log+"\nDone." ,SCRIPT_NAME);
        }
    }
}
