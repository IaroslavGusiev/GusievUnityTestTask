# MONEYTIME SDK INTEGRATION for HIDESEEK

This README will guide you to integrate the various SDK required for this UNITY project to work as expected by HideSeek for Android.

It will help you to:
- integrate Applovin MAX Mediation
- integrate the various Firebase required functionnalities (Analytics, Crashlytics, RemoveConfig)
- integrate Google Play
- setup the project in Unity
- build the project



## Prerequisite
- UNITY HUB
- UNITY 2022.3.13f1 to make sure the compatibility is optimal
- Android Studio Support Package when installing Unity



## STEP 1: GENERATE THE UNITY PROJECT

The repository includes:

- "_IntegrationPackage" containing functionnalities and plugins common to every HideSeek game:
    - MoneyTime management
    - Ads Management
    - Coffee Break
    - Data Save system
    - ...

- a second package named after the game project, and containing all the assets and code for the game itself.

- the FireBase Google-Info.plist file as well as the User.Keystore file that are required to build and upload the game.

- a modified "AndroidManifest.xml" file inside the "Plugins/Android/" folder required to open the "MoneyTime" app from the game.

- Once the repository is downloaded:  
    - Open UnityHub
    - Go to Projects
    - Add the project folder to the projects list
    - Make sure the 2022.3.13f1 version of Unity is installed, then open the project

Unity should automatically generate the required Libraries.



## STEP 2: ADDITIONAL UNITY PACKAGES IMPORT

You will find all the required additional packages at the following [github address](https://github.com/InkedLau/HideSeek_Packages).  

For every package, process as follow:
- In Unity, go to Assets
- Import Package
- Custom Package... 
- Then select the ".unitypackage" file you want to import.

When the Import window shows up, click "Import" at the bottom right corner and wait for Unity to integrate the files to the project. 
It may take a couple of minutes depending on the package.



## STEP 3: UNITY SCENES AND SETTINGS

Once every package is imported: 
- drag and drop the scene "UI_Scene" from the "Assets/_IntegrationPackage" folder into the Unity Hierarchy.
- Add the game Scene into the Hierarchy, and unload them by right clicking on them and selecting "Unload Scene". 
> [!NOTE]
> Most of the time, you will find the various game scenes in the folder "Assets/_GameName/Scene"

- In the Hierarchy, unfold the UI_Scene and select the gameobject "SETTINGS".

- In the Inspector, fill in the following information if they are missing:
    - URL Privacy: https://www.hideseek.games/privacy-policies
    - Scene Name: The name of the first game scene that will be loaded after the Hideseek SDK is initialized.
    - Max Sdk Key: The MAX SDK key of the HideSeek Account, available on [AppLovin](https://dash.applovin.com/o/account?r=2#keys) or on the following [Google Spritesheet](https://docs.google.com/spreadsheets/d/1pVgJ9nfU15yF3eF9PjyWYvyGiYSDdE-fKXjwaDAtYrc/edit#gid=0).
    - Admob Android ID: available on the following [Google Spritesheet](https://docs.google.com/spreadsheets/d/1pVgJ9nfU15yF3eF9PjyWYvyGiYSDdE-fKXjwaDAtYrc/edit#gid=0).
    - Max BANNER_ID: available on [AppLovin](https://dash.applovin.com/o/mediation/ad_units/976338179) or on the following [Google Spritesheet](https://docs.google.com/spreadsheets/d/1pVgJ9nfU15yF3eF9PjyWYvyGiYSDdE-fKXjwaDAtYrc/edit#gid=0).
    - Max INTER_ID: available on [AppLovin](https://dash.applovin.com/o/mediation/ad_units/976338179) or on the following [Google Spritesheet](https://docs.google.com/spreadsheets/d/1pVgJ9nfU15yF3eF9PjyWYvyGiYSDdE-fKXjwaDAtYrc/edit#gid=0).
    - Max RW_ID: available on [AppLovin](https://dash.applovin.com/o/mediation/ad_units/976338179) or on the following [Google Spritesheet](https://docs.google.com/spreadsheets/d/1pVgJ9nfU15yF3eF9PjyWYvyGiYSDdE-fKXjwaDAtYrc/edit#gid=0).
    - MoneyTime Game Name: available on the following [Google Spritesheet](https://docs.google.com/spreadsheets/d/1pVgJ9nfU15yF3eF9PjyWYvyGiYSDdE-fKXjwaDAtYrc/edit#gid=0).

Once done, don't forget to save the UI Scene. You can also override the "SETTINGS" prefab by selecting the SETTINGS GameObject in the scene, then selecting "override" on the top left corner of the Inspector window.



## STEP 4: APPLOVIN MAX SDK SETUP

Everything code-related is already implemented inside the script "Assets/_IntegrationPackage/Script/ManagerAds.cs"

Every additional instruction to complete the Applovin MAX integration is available at this address:  https://dash.applovin.com/documentation/mediation/unity/getting-started/integration
You will mainly have to manually select the various mediation available in the Applovin MAX mediation window in Unity:
- Select Applovin in the top menu
- Select Integration Manager

Then don't forget to fill in the GoogleAdManager APP ID as well as the MAX SDK KEY, both of them available on the following [Google Spritesheet](https://docs.google.com/spreadsheets/d/1pVgJ9nfU15yF3eF9PjyWYvyGiYSDdE-fKXjwaDAtYrc/edit#gid=0).




## STEP 5: BUILD SETUP

- Go to File
- Build Settings
- Delete any existing Scene in Build
- Add Open Scenes
- Select the Android Platform
- Switch Platform
- [x] Tick the option "Build App Bundle"
- Select "Player Settings"


### In the Project Settings:
- Resolution and Presentation:
    - Orientation -> Default Orientation -> Portrait

- Splash Image:
    - [ ] Untick "Show Unity Logo"
    - Add the HideSeek logo in the logos list, available in "Assets/_IntegrationPackage/Sprites/HS_1024.png"
    - Set the Background Color to Black `#000000`

- Other Settings:
    - Identification: 
        - Enable Override Default Package Name if needed
        - Fill in the Package Name if needed (com.hideseek.gamename)
        - Setup a new version and new bundle version code
        - Make sure the Minimum API Level and Target API Level are high enough depending on the SDK Version
    
    - Script Compilation -> Scripting Define Symbols -> Add the Following:
        - GOOGLE_PLAY
        - MAX
        - FB
    - Apply
> [!CAUTION]
> IF THE DEFINE SYMBOLS ARE NOT ADDED, THE ADS WILL NOT RUN

- Publishing settings
    - Enable Custom Keystore
    - Select the file "user.keystore" available at the root folder of the project
    - Fill in the password
    - Select the right Alias
    - Fill in the Project Key password


    - Go to Buid:
        - [x] Enable "Custom Main Manifest" if not already selected (It should already be included in the project folder)
        - [x] Enable "Custom Main Gradle Template"
        - [x] Enable "Custom Gradle Properties Template"
        - [x] Enable "Custom Gradle Settings Template"

> [!CAUTION]
> IF THE BUILD TEMPLATES ARE NOT SETUP CORRECTLY, THE DEPENDENCY RESOLVER WILL FAIL AND THE PROJECT WILL NOT BUILD



## STEP 6: FINAL SETUP AND CHECK BEFORE BUILDING

- Open the file "AndroidManifest.xml" in the folder "Assets/Plugins/Android" and make sure the file contains the following code:

```
<?xml version="1.0" encoding="utf-8"?>
<manifest
    xmlns:android="http://schemas.android.com/apk/res/android"
    package="com.unity3d.player"
    xmlns:tools="http://schemas.android.com/tools">

    <queries>
        <package android:name="com.money.time"/>
    </queries>

    <application>
        <activity android:name="com.unity3d.player.UnityPlayerActivity"
                  android:theme="@style/UnityThemeSelector">
            <intent-filter>
                <action android:name="android.intent.action.MAIN" />
                <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
        </activity>
        <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
    </application>
</manifest>
```

Copy and paste this code inside the AndroidManifest.xml if necessary.


Once done, go back to Unity:
- Go to "Assets"
- External Dependency Manager
- Android Resolver
- Force Resolve

A window should appear notifying that the Resolution has Succeeded.

> [!TIP]
> If the resolution fails, close Unity as well as Visual Code / Visual Studio Code. At the root of the project, delete the file "Library", then open again the project in Unity and try to do a new force resolve. Most of the time, it is enough to solve the Dependency Resolver issue.



## STEP 7: BUILD

In Unity:
- Go to "File"
- Build Settings
- Build

If every step has been followed, you should obtain a ".aab" file that could will be able to upload on the Android Dev Console.

Congratulations!