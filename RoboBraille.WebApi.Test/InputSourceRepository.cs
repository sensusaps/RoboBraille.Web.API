using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Test
{
    public class InputSourceRepository
    {
        public static string GetTestResultDirectory()
        {
            return ConfigurationManager.AppSettings.Get("ResultDirectory");
        }

        public static string GetTestDirectory()
        {
            return ConfigurationManager.AppSettings.Get("TestDirectory");
        }

        public InputSourceRepository()
        {
        }

        public string GetTestForLanguage(string langauge)
        {
            return languageText[langauge];
        }


        private static readonly Dictionary<string, string> languageText = new Dictionary<string, string>()
        {
            {"enUS", @"Follow the four easy steps below to have your document converted into an alternative, accessible format. The result is delivered in your email inbox. You may upload one or more files, enter a URL to a file or simply type in the text you wish to have converted. The form expands as you make your selections."},
            {"enGB", @"Follow the four easy steps below to have your document converted into an alternative, accessible format. The result is delivered in your email inbox. You may upload one or more files, enter a URL to a file or simply type in the text you wish to have converted. The form expands as you make your selections."},
            {"enAU", @"Follow the four easy steps below to have your document converted into an alternative, accessible format. The result is delivered in your email inbox. You may upload one or more files, enter a URL to a file or simply type in the text you wish to have converted. The form expands as you make your selections."},
            {"daDK", @"Følg de fire enkle trin nedenfor for at konvertere dit dokument til et alternativt, tilgængeligt format. Du modtager resultatet via email i din indbakke. Du kan uploade en eller flere filer, angive en URL til en fil eller taste teksten direkte ind. Formularen folder sig ud efterhånden som du udfylder den."},
            {"bgBG", @"Следвайте четирите лесни стъпки за да преобразувате Вашия документ в алтернативен, достъпен формат. Резултатът ще бъде доставен на Вашата електронна поща. Документът се разширява като добавяте Вашите предпочитания."},
            {"frFR", @"Suivez les quatre étapes suivantes pour convertir votre document dans un autre format accessible. Le résultat vous sera envoyé par email. Le formulaire se développe au fur et á mesure que vous effectuez vos sélections."},
            {"deDE", @"Führen Sie die vier einfachen unten angegebenen Schritte aus, um ein Dokument in ein Alternativformat zu konvertieren. Das konvertierte Dokument wird an Ihre E-Mail-Adresse gesendet. Sie können eine oder mehrere Dateien hochladen, einen Link zu einem Dokument hinzufügen oder einfach den Text eintippen, den Sie konvertiert haben möchten. Sobald Sie eine Auswahl im Eingabeformular getroffen haben, werden weitere Optionen zur Auswahl angezeigt."},
            {"klKL", @"Illit allakkiat format-imut taartaa-sussamut iserfigiuminartumut konvertererniarlugu periusissat matuma ataani allaqqasut pisariit-sut malikkit. Taakkua inerneri illit email-ivit indbakkiatigut tigussa-vatit. Fili uploadersinnaavat, URL filinngortissinnaat, oqaasertassaa-lu toqqaannartumik allassinnaal-lugit. Formularip immersugassar-taanik immersuinerit ilutigalugu immersugassatit saqqummertas-sapput."},
            {"huHU", @"Hajtsa végre az alábbi négy egyszerű lépést, amennyiben akadálymentes formára szeretne konvertáltatni egy dokumentumot. Az eredményt az Ön által megadott e-mail címre küldjük vissza. Kitöltés közben az űrlap szerkezete a beállításoktól függően megváltozik."},
            {"itIT", @"Segui i quattro semplici passi seguenti per convertire il tuo documento in un formato alternativo accessibile. Il risultato verrà consegnato nella tua casella email. Questo modulo si espande man mano che operi le tue scelte."},
            {"plPL", @"Wykonaj cztery proste kroki, opisane poniżej, aby Twój dokument został przekształcony na wybrany dostępny format. Rezultat zostanie przesłany na Twoją skrzynkę pocztową. Formularz rozwija się w miarę dokonywania właściwych wyborów."},
            {"ptPT", @"Siga quatro simples passos para converter o seu ficheiro para um formato alternativo acessível. O resultado será enviado para a sua caixa de correio eletrónico. O formulário irá adaptar-se à medida que o for preenchendo."},
            {"roRO", @"Urmaţi cei patru paşi simpli de mai jos pentru a avea documentul convertit într-un format alternativ, accesibil. Rezultatul este livrat prin e-mail. Formularul se extinde pe măsură ce faceţi selecţiile dorite."},
            {"slSI", @"Sledite štirim preprostim korakom spodaj, da prevedete svoj dokument v drugače dostopen zapis. Rezultat boste prejeli v svoj e-poštni nabiralnik. Obrazec se bo razširjal, ko boste izbirali."},
            {"esES", @"Siga los cuatro pasos que se muestran a continuación para convertir su documento a un formato alternativo y accesible. El documento transformado será enviado a su casilla de correo. El formulario se expande a medida que usted selecciona las distintas opciones."},
            {"esMX", @"Siga los cuatro pasos que se muestran a continuación para convertir su documento a un formato alternativo y accesible. El documento transformado será enviado a su casilla de correo. El formulario se expande a medida que usted selecciona las distintas opciones."},
            {"isIS", @"Fylgdu eftirfarandi skrefum til að breyta skránni í aðgengilegt skráarsnið fyrir punktaletur. Niðustaðan er síðan send í pósthólfið þitt. Athugið að síðan uppfærist þegar hlutur er valinn."},
            {"zhCN", @"按照以下四個簡單步驟，將您的文檔轉換為替代的可訪問格式。 結果將在您的電子郵件收件箱中發送。 您可以上傳一個或多個文件，輸入文件的URL或只需鍵入要轉換的文本。 表單會在您進行選擇時展開。"},
            {"zhHK", @"按照以下四个简单步骤，将您的文档转换为替代的可访问格式。 结果将在您的电子邮件收件箱中发送。 您可以上传一个或多个文件，输入文件的URL或只需键入要转换的文本。 表单会在您进行选择时展开。"},
            {"zhTW", @"按照以下四个简单步骤，将您的文档转换为替代的可访问格式。 结果将在您的电子邮件收件箱中发送。 您可以上传一个或多个文件，输入文件的URL或只需键入要转换的文本。 表单会在您进行选择时展开。"},
            {"nlNL", @"Volg de onderstaande vier eenvoudige stappen uw document te hebben omgezet in een alternatief toegankelijk formaat. Het resultaat wordt geleverd in uw e-mail inbox. U kunt een of meer bestanden te uploaden, geeft u een URL naar een bestand of typt u de tekst die u wenst te hebben omgezet. De vorm breidt als je je selecties te maken."},
            {"arAR", @"اتبع الخطوات الأربع سهلة دون أن يكون المستند تحويلها إلى بديل، شكل يسهل الاطلاع عليه. يتم تسليم النتيجة في صندوق بريدك الإلكتروني. تستطيع تحميل ملف واحد أو أكثر، أدخل عنوان URL إلى ملف أو ببساطة اكتب في النص الذي ترغب في تحولوا. يوسع النموذج إجراء التحديدات."},
            {"arENBL", @"اتبع الخطوات الأربع سهلة دون أن يكون المستند تحويلها إلى بديل، شكل يسهل الاطلاع عليه. يتم تسليم النتيجة في صندوق بريدك الإلكتروني. تستطيع تحميل ملف واحد أو أكثر، أدخل عنوان URL إلى ملف أو ببساطة اكتب في النص الذي ترغب في تحولوا. يوسع النموذج إجراء التحديدات."},
            {"fiFI", @"Seuraa neljä helppo ohjeiden ovat asiakirjan muunnetaan vaihtoehto, helposti saatavilla olevassa muodossa. Tulos toimitetaan sähköpostiisi. Voit ladata yhden tai useamman tiedoston, anna tiedoston URL-osoitteen tai yksinkertaisesti kirjoittaa tekstiä haluat muuntanut. Lomake laajenee kuin teet valintoja."},
            {"elGR", @"Ακολουθήστε τα τέσσερα εύκολα βήματα παρακάτω, για να έχουν το έγγραφό σας μετατρέπεται σε μια εναλλακτική, προσβάσιμη μορφή. Το αποτέλεσμα παραδίδεται στα εισερχόμενα e-mail σας. Μπορείτε να ανεβάσετε ένα ή περισσότερα αρχεία, εισάγετε μια διεύθυνση URL σε ένα αρχείο ή απλά πληκτρολογήστε το κείμενο που θέλετε να μετατραπεί. Η μορφή επεκτείνεται, όπως μπορείτε να κάνετε τις επιλογές σας."},
            {"jaJP", @"次の4つの簡単な手順に従って、文書を別のアクセス可能な形式に変換します。 結果はあなたのEメール受信箱に届きます。 1つまたは複数のファイルをアップロードしたり、URLをファイルに入力したり、変換したいテキストを入力したりすることができます。 選択したフォームは展開されます。"},
            {"koKR", @"아래의 네 가지 단계를 따르면 문서를 액세스 가능한 대체 형식으로 변환 할 수 있습니다. 결과는 이메일받은 편지함에 전달됩니다. 하나 이상의 파일을 업로드하거나 URL을 파일에 입력하거나 단순히 변환하려는 텍스트를 입력 할 수 있습니다. 선택한 양식이 펼쳐집니다."},
            {"ltLT", @"Sekite keturis paprastus veiksmus žemiau, kad jūsų dokumentas paverčiamas alternatyva, prieinama forma. Rezultatas yra pristatomas į jūsų elektroninio pašto dėžutę. Galite įkelti vieną ar daugiau failų, įveskite URL į failą arba tiesiog įrašykite tekstą, kurį norite atsivertė. Formoje plečiasi, kaip jūs darote savo pasirinkimus."},
            {"nnNO", @"Følg de fire enkle trinnene nedenfor for å ha dokumentet konverteres til et alternativ tilgjengelig format. Resultatet blir levert i innboksen din. Du kan laste opp en eller flere filer, legge inn en URL til en fil eller bare skrive inn teksten du ønsker å ha konvertert. Skjemaet utvides som du gjør dine valg."},
            {"ruRU", @"Выполните четыре простых шага ниже, чтобы ваш документ преобразуется в альтернативный, доступный формат. Результат поставляется в своем почтовом ящике. Вы можете загрузить один или несколько файлов, введите URL в файл или просто ввести текст, который вы хотите преобразовали. Форма расширяется, как вы сделаете свой выбор."},
            {"svSE", @"Följ de fyra enkla stegen nedan för att ha ditt dokument omvandlas till ett alternativ, lättillgängligt format. Resultatet levereras i din e-postbrevlåda . Du kan ladda upp en eller flera filer, ange en webbadress till en fil eller helt enkelt skriva in den text du vill ha konverteras. Formuläret expanderar när du gör dina val."},

    };
    }
}
