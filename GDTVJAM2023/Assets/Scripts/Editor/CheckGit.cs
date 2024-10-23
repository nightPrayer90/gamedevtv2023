using System.Threading.Tasks;
using CandyCoded.GitStatus;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace NionForge {
    [InitializeOnLoad]
    public class CheckGit {
        [DoNotSerialize]
        private static bool _hasRunThisTime;
        private static bool _pullNeeded;

        static CheckGit() {
            if (_hasRunThisTime) {
                return;
            }

            Debug.Log("Up and running");
            Task.Run(async () => {
                _pullNeeded = await GitStatus.PullNeeded();
                _hasRunThisTime = true;
            }).ContinueWith(_ => {
                    if (_pullNeeded) {
                        Debug.Log(":(");
                        if (EditorUtility.DisplayDialog("Changes in git repository",
                                "There are changes in the git repository. You should pull first. Close Unity Editor now? (Unsaved changes will be lost)",
                                "Yes", "No")) {
                            // close Unity
                            EditorApplication.Exit(0);
                        }
                    }
                    else {
                        Debug.Log(":)");
                    }
                },
                TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}