﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChromeManagedBookmarksEditor.Model;
using System.Windows;
using System.Collections.ObjectModel;

namespace ChromeManagedBookmarksEditor.Helpers
{
    public static class ChromeJSONConverter
    {
        // Async methods to convert ManagedBookmarks object into JSON code
        public static async Task<string> ConvertToJSON(Folder RootFolder)
        {
            string ReturnableJSON = string.Empty;

            await Task.Run(() => 
            {
                ReturnableJSON = _ConvertToJSON(RootFolder);
            });

            return ReturnableJSON;
        }

        private static string _ConvertToJSON(Folder RootFolder)
        {
            string convertedJSON = string.Empty;

            string topLevelName = $"[{{\"toplevel_name\":\"{RootFolder.Name}\"}},";

            convertedJSON = topLevelName;
            //------------------LOOKING FOR A BETTER WAY
            /*Some thinking stuff...
             * 
             * Idea: -> Array of Joined Strings?
             *     : Create an array of formatted strings per folder in order of recursion. 
             *     : Array1[0] = folder1[],folder3[],url,url -- Root folder
             *     : Array1[1] = folder2[],url,url           -- Folder1's content -- note the second folder is the first folder below folder1, not the next 'same-level' folder.
             *     : Array1[2] = url,url                     -- Folder2's content -- note the array index matches the folders position in the recusion.
             *     : Array1[3] = url,url                     -- Folder3's content
             *     : 
             *     : Join the arrays once the contents are constructed
             *     : Something like -> Array[0].replace(folder1[],folder1[Array1[1]]) 
             *     : NOT Array1[0] + Array1[1]
             *     :
             *     : Possible Issues 
             *     :   - Recursion has to be infinitely scalable.
             *     :   - Joining Array1 together to form the completed code will be a bit tricky, since child items can't just be 'tacked' on. 
             *     :     They need to be placed inside the parent item. 
             *     :     Note: Constructing folders with braces [] and then searching for the first folder[] by name should work as long as the content array is constructed in order correctly.
             *     :   - Possibly more ineffient than just building out the code all at once? (old way)
             *     
             */

            ObservableCollection<string> TestOC = new ObservableCollection<string>();

            string GetFolderJSONContent(Folder folder)
            {
                ObservableCollection<string> folderContents = new ObservableCollection<string>();
                if (folder.folders.Count > 0)
                {
                    foreach (Folder subfolder in folder.folders)
                    {
                        folderContents.Add($"{{\"name\":\"{subfolder.Name}\",\"children\":[]}}");
                    }
                }

                if (folder.URLs.Count > 0)
                {
                    foreach (URL url in folder.URLs)
                    {
                        folderContents.Add($"{{\"name\":\"{url.Name}\",\"url\":\"{url.Url}\"}}");
                    }
                }

                string joinedContents = String.Join(",", folderContents);
                if(joinedContents == "") { joinedContents = "EMPTY"; }
                return joinedContents;
            }

            void IterateSubFolders(Folder folder)
            {
                TestOC.Add(GetFolderJSONContent(folder));
                if (folder.folders.Count > 0)
                {
                    foreach (Folder subFolder in folder.folders)
                    {
                        IterateSubFolders(subFolder);
                    }
                }
            }

            //Get RootFolder content
            TestOC.Add(GetFolderJSONContent(RootFolder));

            //iterate RootFolders.folders
            if (RootFolder.folders.Count > 0)
            {
                foreach (Folder subfolder in RootFolder.folders)
                {
                    IterateSubFolders(subfolder);
                }
            }

            //combine/replace content strings here
            for(int i = 0; i < TestOC.Count(); i++)
            {
                /* Just some visual aid
                 * 1  root content
                 * 2  folder 1
                 * 3  folder 1 .. 2
                 * 4  folder 3
                 * 5  folder 4
                 * 6  folder 4 .. 5
                 * 7  folder 4 .. 6
                 * 8  folder 4 .. 7
                 * 9  folder 8
                 * 10 folder 9
                 */
            }

            //just a test sting presention.<<
            string TestString = "";
            for(int i = 0; i < TestOC.Count(); i++)
            {
                TestString += $"{TestOC[i].ToString()} INDEX: {i.ToString()}\n\n";
            }
            MessageBox.Show(TestString);
            //-----------------------------<<



            //------------------OLD WAY BELOW

            //string iterateChildObjects(List<Folder> childFolders, bool IsLastFolder)
            //{

            //    return string.Empty;
            //}

            //for(int i = 0; i < RootFolder.folders.Count(); i++)
            //{
            //    if(i == 0)
            //    {
            //        convertedJSON += $"{{\"name\":\"{RootFolder.folders[i].Name}\",\"children\":[";
            //    }
            //    else
            //    {
            //        convertedJSON += $",{{\"name\":\"{RootFolder.folders[i].Name}\",\"children\":[";

            //    }

            //    convertedJSON += "]}";
            //}

            //for(int i = 0; i < RootFolder.URLs.Count(); i++)
            //{

            //}

            //------------------OLD OLD WAY BELOW

            //    string iterateChildObject(int index, List<Folder> Folders, List<URL> Urls, bool isChildOfChild)
            //    {
            //        string childObjects = String.Empty;

            //        for (int o = 0; o < Folders.Count; o++)
            //        {
            //            if (Folders[o].folders != null || Folders[o].URLs != null)
            //            {
            //                string Name = Folders[o].Name;

            //                if (o is 0)
            //                {
            //                    childObjects += string.Format("{{\"name\":\"{0}\",\"children\":[", Name);
            //                }
            //                else
            //                {
            //                    childObjects += string.Format(",{{\"name\":\"{0}\",\"children\":[", Name);
            //                }

            //                List<Folder> childFolders = new List<Folder>();

            //                if (Folders[o].folders != null)
            //                {
            //                    foreach (Folder folder in Folders[o].folders)
            //                    {
            //                        childFolders.Add(folder);
            //                    }
            //                }

            //                List<URL> childURLs = new List<URL>();

            //                foreach (URL url in Folders[o].URLs)
            //                {
            //                    childURLs.Add(url);
            //                }

            //                string childItems = iterateChildObject(o, childFolders, childURLs, true);

            //                childObjects += childItems;
            //            }
            //        }


            //        for (int n = 0; n < Urls.Count; n++)
            //        {
            //            string urlName = Urls[n].Name.ToString();
            //            string url = Urls[n].Url.ToString();

            //            if (index is 0 && n == 0 || n == 0)
            //            {
            //                childObjects += string.Format("{{\"name\":\"{0}\",\"url\":\"{1}\"}}", urlName, url);
            //            }
            //            else
            //            {
            //                childObjects += string.Format(",{{\"name\":\"{0}\",\"url\":\"{1}\"}}", urlName, url);
            //            }
            //        }

            //        if (isChildOfChild ? index + 1 < Folders.Count || chromeBookmarks[0].Folders[index].URLs.Count != 0 : index + 1 < chromeBookmarks[0].Folders.Count)
            //        {
            //            childObjects += "]},";
            //        }
            //        else
            //        {
            //            childObjects += "]}";
            //        }

            //        return childObjects;
            //    }

            //    for (int i = 0; i < chromeBookmarks[0].Folders.Count; i++)
            //    {
            //        string Name = chromeBookmarks[0].Folders[i].Name.ToString();

            //        if (i is 0 || i + 1 == chromeBookmarks[0].Folders.Count)
            //        {
            //            ConvertedJSON += string.Format("{{\"name\":\"{0}\",\"children\":[", Name);
            //        }
            //        else
            //        {
            //            ConvertedJSON += string.Format(",{{\"name\":\"{0}\",\"children\":[", Name);
            //        }

            //        List<Folder> childFolders = new List<Folder>();


            //        foreach (Folder folder in chromeBookmarks[0].Folders[i].folders)
            //        {
            //            childFolders.Add(folder);
            //        }

            //        List<URL> childURLs = new List<URL>();

            //        foreach (URL url in chromeBookmarks[0].Folders[i].URLs)
            //        {
            //            childURLs.Add(url);
            //        }

            //        string subFolders = iterateChildObject(i, childFolders, childURLs, false);

            //        ConvertedJSON += subFolders;
            //    }


            //    for (int i = 0; i < chromeBookmarks[0].URLs.Count; i++)
            //    {
            //        string urlName = chromeBookmarks[0].URLs[i].Name.ToString();
            //        string url = chromeBookmarks[0].URLs[i].Url.ToString();

            //        if (i is 0 ? chromeBookmarks[0].Folders.Count is 0 : false)
            //        {
            //            ConvertedJSON += string.Format("{{\"name\":\"{0}\",\"url\":\"{1}\"}}", urlName, url);
            //        }
            //        else
            //        {
            //            ConvertedJSON += string.Format(",{{\"name\":\"{0}\",\"url\":\"{1}\"}}", urlName, url);
            //        }
            //    }

            //    ConvertedJSON += "]";

            return convertedJSON;
        }

        // Async Methods to parse JSON code into a ManagedBookmarks object
        public static async Task<ManagedBookmarks> ParseJSON(string JSONCode)
        {
            ManagedBookmarks ReturnableManagedBookmarks = new ManagedBookmarks();

            await Task.Run(() =>
            {
                ReturnableManagedBookmarks = _ParseJSON(JSONCode);
            });

            return ReturnableManagedBookmarks;
        }

        private static ManagedBookmarks _ParseJSON(string JSONCode)
        {
            ManagedBookmarks ParsedBookmarks = new ManagedBookmarks();

            System.Threading.Thread.Sleep(5000);

            return ParsedBookmarks;
        }
    }
}


//All of this non-sense is going to be reworked, just moving it to the correct class for now.

//private async Task<ObservableCollection<ManagedBookmarks>> LoadJSON(string json)
//{
//    ObservableCollection<ManagedBookmarks> bookmarks = new ObservableCollection<ManagedBookmarks>();

//    await Task.Run(() =>
//    {
//        bookmarks = _LoadJSON(json);
//    });

//    return bookmarks;
//}

////this is going to be completly redone in the TreeConverter class
//private ObservableCollection<ManagedBookmarks> _LoadJSON(string json)
//{
//    try
//    {
//        ObservableCollection<ManagedBookmarks> Bookmarks = new ObservableCollection<ManagedBookmarks>();
//        ObservableCollection<Folder> FoldersList = new ObservableCollection<Folder>();

//        var root = JsonConvert.DeserializeObject<ObservableCollection<RootFolder>>(json);
//        var parsedfolders = JsonConvert.DeserializeObject<ObservableCollection<ParsedFolders>>(json);
//        var urls = JsonConvert.DeserializeObject<ObservableCollection<URL>>(json);


//        ObservableCollection<URL> clearNullURLs(ObservableCollection<URL> Urls)
//        {
//            for (int i = 0; i < Urls.Count; i++)
//            {
//                if (Urls[i].Name == null || Urls[i].Url == null)
//                {
//                    Urls.RemoveAt(i);
//                    i--;
//                }
//            }

//            return Urls;
//        }

//        Folder iterateChildObjects(string subJson)
//        {
//            try
//            {
//                var parsedSubfolders = JsonConvert.DeserializeObject<List<ParsedFolders>>(subJson);
//                Folder subfolder = new Folder();
//                Folder newfolder = new Folder();

//                for (int i = 0; i < parsedSubfolders.Count; i++)
//                {
//                    if (parsedSubfolders[i].children != null)
//                    {
//                        if (parsedSubfolders[i].children.ToString().Contains("children"))
//                        {
//                            newfolder = iterateChildObjects(parsedSubfolders[i].children.ToString());
//                        }

//                        if (newfolder.Name != null)
//                        {
//                            subfolder.folders = new ObservableCollection<Folder>();
//                            subfolder.folders.Add(newfolder);
//                        }

//                        subfolder.Name = parsedSubfolders[i].Name.ToString();
//                        subfolder.URLs = JsonConvert.DeserializeObject<ObservableCollection<URL>>(parsedSubfolders[i].children.ToString());

//                        subfolder.URLs = clearNullURLs(subfolder.URLs);
//                    }
//                }

//                return subfolder;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.Message);
//                return null;
//            }

//        }

//        for (int i = 0; i < parsedfolders.Count; i++)
//        {
//            if (parsedfolders[i].children != null)
//            {
//                Folder folder = new Folder();
//                folder.folders = new ObservableCollection<Folder>();

//                if (parsedfolders[i].children.ToString().Contains("children"))
//                {
//                    Folder subfolder = new Folder();
//                    subfolder = iterateChildObjects(parsedfolders[i].children.ToString());
//                    folder.folders.Add(subfolder);
//                }

//                folder.Name = parsedfolders[i].Name.ToString();
//                folder.URLs = JsonConvert.DeserializeObject<ObservableCollection<URL>>(parsedfolders[i].children.ToString());
//                folder.URLs = clearNullURLs(folder.URLs);
//                FoldersList.Add(folder);
//            }
//        }

//        urls = clearNullURLs(urls);

//        Bookmarks.Add(new ManagedBookmarks()
//        {
//            toplevel_name = root[0].toplevel_name.ToString(),

//            Folders = FoldersList,

//            URLs = urls
//        });

//        return Bookmarks;
//    }
//    catch (Exception ex)
//    {
//        MessageBox.Show(ex.Message);
//        return null;
//    }
//}

//private async Task<string> ConvertTreeToJSON(ObservableCollection<ManagedBookmarks> chromeBookmarks)
//{
//    string chromeJSONcode = String.Empty;

//    await Task.Run(() =>
//    {
//        chromeJSONcode = _ConvertTreeToJSON(chromeBookmarks);
//    });

//    return chromeJSONcode;
//}

////this is going to be completly redone in the TreeConverter class
//private string _ConvertTreeToJSON(ObservableCollection<ManagedBookmarks> chromeBookmarks)
//{
//    string ConvertedJSON = String.Empty;

//    string toplevel_name = string.Format("[{{\"toplevel_name\":\"{0}\"}},", chromeBookmarks[0].toplevel_name.ToString());

//    ConvertedJSON = toplevel_name;

//    string iterateChildObject(int index, List<Folder> Folders, List<URL> Urls, bool isChildOfChild)
//    {
//        string childObjects = String.Empty;

//        for (int o = 0; o < Folders.Count; o++)
//        {
//            if (Folders[o].folders != null || Folders[o].URLs != null)
//            {
//                string Name = Folders[o].Name;

//                if (o is 0)
//                {
//                    childObjects += string.Format("{{\"name\":\"{0}\",\"children\":[", Name);
//                }
//                else
//                {
//                    childObjects += string.Format(",{{\"name\":\"{0}\",\"children\":[", Name);
//                }

//                List<Folder> childFolders = new List<Folder>();

//                if (Folders[o].folders != null)
//                {
//                    foreach (Folder folder in Folders[o].folders)
//                    {
//                        childFolders.Add(folder);
//                    }
//                }

//                List<URL> childURLs = new List<URL>();

//                foreach (URL url in Folders[o].URLs)
//                {
//                    childURLs.Add(url);
//                }

//                string childItems = iterateChildObject(o, childFolders, childURLs, true);

//                childObjects += childItems;
//            }
//        }


//        for (int n = 0; n < Urls.Count; n++)
//        {
//            string urlName = Urls[n].Name.ToString();
//            string url = Urls[n].Url.ToString();

//            if (index is 0 && n == 0 || n == 0)
//            {
//                childObjects += string.Format("{{\"name\":\"{0}\",\"url\":\"{1}\"}}", urlName, url);
//            }
//            else
//            {
//                childObjects += string.Format(",{{\"name\":\"{0}\",\"url\":\"{1}\"}}", urlName, url);
//            }
//        }

//        if (isChildOfChild ? index + 1 < Folders.Count || chromeBookmarks[0].Folders[index].URLs.Count != 0 : index + 1 < chromeBookmarks[0].Folders.Count)
//        {
//            childObjects += "]},";
//        }
//        else
//        {
//            childObjects += "]}";
//        }

//        return childObjects;
//    }

//    for (int i = 0; i < chromeBookmarks[0].Folders.Count; i++)
//    {
//        string Name = chromeBookmarks[0].Folders[i].Name.ToString();

//        if (i is 0 || i + 1 == chromeBookmarks[0].Folders.Count)
//        {
//            ConvertedJSON += string.Format("{{\"name\":\"{0}\",\"children\":[", Name);
//        }
//        else
//        {
//            ConvertedJSON += string.Format(",{{\"name\":\"{0}\",\"children\":[", Name);
//        }

//        List<Folder> childFolders = new List<Folder>();


//        foreach (Folder folder in chromeBookmarks[0].Folders[i].folders)
//        {
//            childFolders.Add(folder);
//        }

//        List<URL> childURLs = new List<URL>();

//        foreach (URL url in chromeBookmarks[0].Folders[i].URLs)
//        {
//            childURLs.Add(url);
//        }

//        string subFolders = iterateChildObject(i, childFolders, childURLs, false);

//        ConvertedJSON += subFolders;
//    }


//    for (int i = 0; i < chromeBookmarks[0].URLs.Count; i++)
//    {
//        string urlName = chromeBookmarks[0].URLs[i].Name.ToString();
//        string url = chromeBookmarks[0].URLs[i].Url.ToString();

//        if (i is 0 ? chromeBookmarks[0].Folders.Count is 0 : false)
//        {
//            ConvertedJSON += string.Format("{{\"name\":\"{0}\",\"url\":\"{1}\"}}", urlName, url);
//        }
//        else
//        {
//            ConvertedJSON += string.Format(",{{\"name\":\"{0}\",\"url\":\"{1}\"}}", urlName, url);
//        }
//    }

//    ConvertedJSON += "]";

//    return ConvertedJSON;
//}