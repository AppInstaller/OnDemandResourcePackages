using System;
using System.Collections.Generic;

namespace CoffeeUniversal.Helpers
{
    public class RecognizerHelper
    {
        public static Dictionary<string, string> RecognizerNames { get; set; }

        static RecognizerHelper()
        {
            if (RecognizerNames == null)
            {
                RecognizerNames = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

                RecognizerNames.Add("en-US", "Microsoft English (US) Handwriting Recognizer");
                RecognizerNames.Add("en-GB", "Microsoft English (UK) Handwriting Recognizer");
                RecognizerNames.Add("en-CA", "Microsoft English (Canada) Handwriting Recognizer");
                RecognizerNames.Add("en-AU", "Microsoft English (Australia) Handwriting Recognizer");
                RecognizerNames.Add("de-DE", "Microsoft-Handschrifterkennung - Deutsch");
                RecognizerNames.Add("de-CH", "Microsoft-Handschrifterkennung - Deutsch (Schweiz)");
                RecognizerNames.Add("es-ES", "Reconocimiento de escritura a mano en español de Microsoft");
                RecognizerNames.Add("es-MX", "Reconocedor de escritura en Español (México) de Microsoft");
                RecognizerNames.Add("es-AR", "Reconocedor de escritura en Español (Argentina) de Microsoft");
                RecognizerNames.Add("fr", "Reconnaissance d'écriture Microsoft - Français");
                RecognizerNames.Add("fr-FR", "Reconnaissance d'écriture Microsoft - Français");
                RecognizerNames.Add("ja", "Microsoft 日本語手書き認識エンジン");
                RecognizerNames.Add("ja-JP", "Microsoft 日本語手書き認識エンジン");
                RecognizerNames.Add("it", "Riconoscimento grafia italiana Microsoft");
                RecognizerNames.Add("nl-NL", "Microsoft Nederlandstalige handschriftherkenning");
                RecognizerNames.Add("nl-BE", "Microsoft Nederlandstalige (België) handschriftherkenning");
                RecognizerNames.Add("zh", "Microsoft 中文(简体)手写识别器");
                RecognizerNames.Add("zh-CN", "Microsoft 中文(简体)手写识别器");
                RecognizerNames.Add("zh-Hans-CN", "Microsoft 中文(简体)手写识别器");
                RecognizerNames.Add("zh-Hant", "Microsoft 中文(繁體)手寫辨識器");
                RecognizerNames.Add("zh-TW", "Microsoft 中文(繁體)手寫辨識器");
                RecognizerNames.Add("ru", "Microsoft система распознавания русского рукописного ввода");
                RecognizerNames.Add("pt-BR", "Reconhecedor de Manuscrito da Microsoft para Português (Brasil)");
                RecognizerNames.Add("pt-PT", "Reconhecedor de escrita manual da Microsoft para português");
                RecognizerNames.Add("ko", "Microsoft 한글 필기 인식기");
                RecognizerNames.Add("pl", "System rozpoznawania polskiego pisma odręcznego firmy Microsoft");
                RecognizerNames.Add("sv", "Microsoft Handskriftstolk för svenska");
                RecognizerNames.Add("cs", "Microsoft rozpoznávač rukopisu pro český jazyk");
                RecognizerNames.Add("da", "Microsoft Genkendelse af dansk håndskrift");
                RecognizerNames.Add("nb", "Microsoft Håndskriftsgjenkjenner for norsk");
                RecognizerNames.Add("nn", "Microsoft Håndskriftsgjenkjenner for nynorsk");
                RecognizerNames.Add("fi", "Microsoftin suomenkielinen käsinkirjoituksen tunnistus");
                RecognizerNames.Add("ro", "Microsoft recunoaştere grafie - Română");
                RecognizerNames.Add("hr", "Microsoftov hrvatski rukopisni prepoznavač");
                RecognizerNames.Add("sr-Latn", "Microsoft prepoznavač rukopisa za srpski (latinica)");
                RecognizerNames.Add("sr", "Microsoft препознавач рукописа за српски (ћирилица)");
                RecognizerNames.Add("ca", "Reconeixedor d'escriptura manual en català de Microsoft");
            }
        }
    }
}
