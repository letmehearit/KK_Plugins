﻿using System;
using System.IO;
using System.Linq;
using XUnity.AutoTranslator.Plugin.Core;
using XUnity.AutoTranslator.Plugin.Core.AssetRedirection;
using XUnity.AutoTranslator.Plugin.Core.Utilities;
using XUnity.ResourceRedirector;
using BepInEx.Logging;



namespace KK_Plugins
{
    public class ExcelDataResourceRedirector : AssetLoadedHandlerBaseV2<ExcelData>
    {
        private static ManualLogSource Logger => TextResourceRedirector.Logger;

        public ExcelDataResourceRedirector() => CheckDirectory = true;

        protected override string CalculateModificationFilePath(ExcelData asset, IAssetOrResourceLoadedContext context) =>
            context.GetPreferredFilePathWithCustomFileName(asset, null).Replace(".unity3d", "");

        protected override bool DumpAsset(string calculatedModificationPath, ExcelData asset, IAssetOrResourceLoadedContext context)
        {
            var defaultTranslationFile = Path.Combine(calculatedModificationPath, "translation.txt");
            var cache = new SimpleTextTranslationCache(
               file: defaultTranslationFile,
               loadTranslationsInFile: false);

            foreach (var param in asset.list)
            {
                for (int i = 0; i < param.list.Count; i++)
                {
                    var key = param.list[i];
                    if (!string.IsNullOrEmpty(key) && LanguageHelper.IsTranslatable(key))
                    {
                        cache.AddTranslationToCache(key, key);
                    }
                }
            }

            return true;
        }

        protected override bool ReplaceOrUpdateAsset(string calculatedModificationPath, ref ExcelData asset, IAssetOrResourceLoadedContext context)
        {
            var defaultTranslationFile = Path.Combine(calculatedModificationPath, "translation.txt");
            var redirectedResources = RedirectedDirectory.GetFilesInDirectory(calculatedModificationPath, ".txt");
            var streams = redirectedResources.Select(x => x.OpenStream());
            var cache = new SimpleTextTranslationCache(
               outputFile: defaultTranslationFile,
               inputStreams: streams,
               allowTranslationOverride: false,
               closeStreams: true);

            foreach (var param in asset.list)
            {
                for (int i = 0; i < param.list.Count; i++)
                {
                    var key = param.list[i];
                    if (!string.IsNullOrEmpty(key))
                    {
                        if (cache.TryGetTranslation(key, true, out var translated))
                        {
                            param.list[i] = translated;
                        }
                        else if (AutoTranslatorSettings.IsDumpingRedirectedResourcesEnabled && LanguageHelper.IsTranslatable(key))
                        {
                            cache.AddTranslationToCache(key, key);
                        }
                    }
                }
            }

            return true;
        }

        protected override bool ShouldHandleAsset(ExcelData asset, IAssetOrResourceLoadedContext context) => !context.HasReferenceBeenRedirectedBefore(asset);
    }
}