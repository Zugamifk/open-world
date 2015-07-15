using UnityEngine;
using System.Collections;
using System.IO;
using System;

/** Utility class for locating various files and directories of interest.
	The class is partial so that additional path strings can be included as required.
 */

public partial class Paths {
	
	public const string kTextAssetDir = "Assets/TextAssets/",
						kBuildSettingsResourcesDir = "Build Settings/",
						kBuildSettingsDir = "Assets/Resources/" + kBuildSettingsResourcesDir;
	
	private static string m_appDataPath;
	/** Returns the path for the directory where the Application should write data at runtime (save games, etc.) */
	public static string GameAppDataPath {
		get {
			if (m_appDataPath == null) {
				switch (Application.platform) {
					case RuntimePlatform.WindowsPlayer:
					case RuntimePlatform.WindowsEditor:
					{
						string generalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
						// XP: C:\Documents and Settings\<username>\Application Data
						// Vista: C:\Users\<username>\AppData\Roaming
						m_appDataPath =	generalAppDataPath + Path.DirectorySeparatorChar +
									VersionInfo.CompanyName + Path.DirectorySeparatorChar + VersionInfo.ProductName;
					}
						break;
					case RuntimePlatform.OSXPlayer:
					case RuntimePlatform.OSXEditor:
					{
						string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
						var directoryInfo = new DirectoryInfo(desktopPath);
						string generalAppDataPath = directoryInfo.Parent + "/Library/Application Support";
						// /Users/username/Library/Application Support
						try {
							using (var infoPlistReader = new StreamReader(Path.Combine(Application.dataPath, "Info.plist"))) {
								while (infoPlistReader.Peek() != -1) {
									string line = infoPlistReader.ReadLine();
									if (line.Contains("CFBundleIdentifier")) {
										line = infoPlistReader.ReadLine();
										int	begin = line.IndexOf('>') + 1,
											length = line.LastIndexOf('<') - begin;
										m_appDataPath = generalAppDataPath + Path.DirectorySeparatorChar + line.Substring(begin, length);
										break;
									}
								}
							}
							if (m_appDataPath == null)  {
								throw new Exception("No CFBundleIdentifier found");
							}
						} catch (Exception e) {
							if (Application.platform != RuntimePlatform.OSXEditor) {
								Debug.LogWarning("Could not read CFBundleIdentifier from Info.plist. Using Company/Product data path instead.\n" + e);
							}
							m_appDataPath =	generalAppDataPath + Path.DirectorySeparatorChar +
										VersionInfo.CompanyName + Path.DirectorySeparatorChar + VersionInfo.ProductName;
						}
					}
						break;
					case RuntimePlatform.IPhonePlayer:
					{
						m_appDataPath = Application.persistentDataPath;								
					}
						break;
					case RuntimePlatform.Android:
					{
						m_appDataPath = Application.persistentDataPath;
					}
						break;
					default:
						throw new Exception("Unknown platform: no specified datapath for " + Application.platform);
				}
				Directory.CreateDirectory(m_appDataPath);
				m_appDataPath += Path.DirectorySeparatorChar;
			}
			return m_appDataPath;
		}
	}
	
	/** Returns the path where streaming assets are stored. */
	public static string StreamingAssetsPath {
		get {
			if (m_streamingAssetsPath == null) {
#if UNITY_EDITOR
					m_streamingAssetsPath = Application.dataPath + "/StreamingAssets";
#elif UNITY_ANDROID
			 		m_streamingAssetsPath = "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
			 		m_streamingAssetsPath = Application.dataPath + "/Raw";
#else
					if (Application.platform == RuntimePlatform.OSXPlayer) {
			 			m_streamingAssetsPath = Application.dataPath + "/Data/StreamingAssets";
					} else if (Application.platform == RuntimePlatform.WindowsPlayer) {
			 			m_streamingAssetsPath = Application.dataPath + "/StreamingAssets";
					} else {
			 			m_streamingAssetsPath = Application.dataPath + "/StreamingAssets";
						Debug.LogWarning("Unsure of StreamingAssets location for " + Application.platform + ". Defaulting to " + m_streamingAssetsPath);
					}
#endif				
			}
			return m_streamingAssetsPath;
		}
	}
	protected static string m_streamingAssetsPath;

}
