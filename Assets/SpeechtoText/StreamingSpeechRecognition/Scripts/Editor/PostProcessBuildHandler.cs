using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using System.IO;
#endif

public class PostProcessBuildHandler
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.iOS)
        {
#if UNITY_IOS
            var projectPath = PBXProject.GetPBXProjectPath(path);
            var project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));
            var unityFrameworkGuid = project.GetUnityFrameworkTargetGuid();
            var mainGuid = project.GetUnityMainTargetGuid();

            // libz.tbd for grpc ios build
            project.AddFrameworkToProject(unityFrameworkGuid, "libz.tbd", false);

            // libgrpc_csharp_ext missing bitcode. as BITCODE exand binary size to 250MB.
            project.SetBuildProperty(unityFrameworkGuid, "ENABLE_BITCODE", "NO");
            project.SetBuildProperty(mainGuid, "ENABLE_BITCODE", "NO");

            File.WriteAllText(projectPath, project.WriteToString());
#endif
        }
    }
}