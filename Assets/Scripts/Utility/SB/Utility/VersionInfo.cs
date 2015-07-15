using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

/** Contains product version information for access at runtime.
	Most properties are only settable in the editor because they are meant
	to be set at build time using information from PlayerSettings and
	the version control system.
	As a build-settings container, there only needs to be one of these
	objects.
	Note that since values are only updated at build-time, playing in the
	editor will use the values from the most recent build.
*/
public class VersionInfo : BuildSettings<VersionInfo> {
	
#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern string _GetCFBundleVersion();
	
	[DllImport("__Internal")]
	private static extern string _GetCFBundleIdentifier();
#endif
	
	public static string CompanyName {
		get {
			return Instance.m_companyName;
		}
#if UNITY_EDITOR
		set {
			Instance.m_companyName = value;
			EditorUtility.SetDirty(Instance);
		}
#endif
	}
	
	public static string ProductName {
		get {
			return Instance.m_productName;
		}
#if UNITY_EDITOR
		set {
			Instance.m_productName = value;
			EditorUtility.SetDirty(Instance);
		}
#endif
	}
	
	//Name of the build target used to build
	public static string TargetName {
		get {
			return Instance.m_targetName;
		}
#if UNITY_EDITOR
		set {
			Instance.m_targetName = value;
			EditorUtility.SetDirty(Instance);
		}
#endif
	}
	
	//Revision number of the working copy used to build
	public static string Revision {
		get {
			return Instance.m_revision;
		}
#if UNITY_EDITOR
		set {
			Instance.m_revision = value;
			if(value.EndsWith("+")){
				Instance.m_intRevision = int.Parse(value.Substring(0, value.Length-1));
			}
			else{
				Instance.m_intRevision = int.Parse(value);
			}
			EditorUtility.SetDirty(Instance);
		}
#endif
	}
	
	public static int IntRevision {
		get {
			return Instance.m_intRevision;
		}
	}

#if UNITY_IPHONE
	/** Public-facing Application version, used on the app store, etc.
		Unlike most settings, this is fetched from the
		info.plist at run-time using a native call. */
	public static string BundleVersion {
		get {
			if (m_bundleVersion == null) {
				#if !UNITY_EDITOR
				m_bundleVersion = _GetCFBundleVersion();
				#else
				m_bundleVersion = "1.0";
				#endif
			}
			return m_bundleVersion;
		}
	}
	protected static string m_bundleVersion;
	
	public static string BundleIdentifier {
		get{
			if(m_bundleIdentifier == null){
				#if !UNITY_EDITOR
				m_bundleIdentifier = _GetCFBundleIdentifier();
				#else
				m_bundleIdentifier = PlayerSettings.iPhoneBundleIdentifier;
				#endif
			}
			return m_bundleIdentifier;
		}
	}
	protected static string m_bundleIdentifier;
#endif
	
	[SerializeField]
	protected string	m_companyName = "Company Name",
						m_productName = "Product Name",
						m_targetName = "Target Name",
						m_revision = "0";
	[SerializeField]
	protected int 		m_intRevision = 0;
	
#if UNITY_EDITOR
	protected override void SetupDefaults(){
		m_companyName = PlayerSettings.companyName;
		m_productName = PlayerSettings.productName;
	}
#endif

}