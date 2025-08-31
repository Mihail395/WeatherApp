# IMPORTANT READ FIRST!
**По вклучувуње во Visual Studio, доколку се појави Error кој што покажува дека некој Packages не се инсталирани ве молам притиснете десен клик на Solution во Solution Explorer-от и на Restore NuGet Packages. Кај некој се случува проблем Visual Studio да не ги врати и вчита сам потребните пакети од Frameworks кој се користат во апликацијата иако во packages config files и .csproject тие се наведени како top level dependencies.** / **Regarding Visual Studio, if an error appears indicating that some packages are not installed, please right-click on the Solution in the Solution Explorer and select Restore NuGet Packages. For some users, there is an issue where Visual Studio does not automatically restore and load the required packages from the frameworks used in the application, even though they are listed as top-level dependencies in the packages.config files and the .csproj files.**

# WeatherApp
**MK** / EN
Апликацијата е basic имплементација на типична апликација за временска прогноза. Има многу едноставен начин на користење кoј што секој корисник може многу лесно да го разбере. Содржи картички на кој се гледаат информации за времето во даден час , исто така и график на температурите во дадени часови.

## Користење
Апликацијата се користи многу едноставно така што при стартување се вклучува прозорецот на кој само се гледаат полето каде што треба да се внесе името на градот и едно копче за барање (Search). Го внесувате името на градот за кој сакате да пребарате и потоа притискате на копчето и чекате. <br>
### [[Линк](https://www.youtube.com/watch?v=QxtZU5X-YSw)] до видео во кое детално се објаснети сите функционалности и како таа се користи.

<img width="912" height="689" alt="Screenshot 2025-08-28 001449" src="https://github.com/user-attachments/assets/45be684d-feb0-431b-918f-30b69c86e32d" />

По добивање на податоците од API повикот , графикот и картичките се пополнуваат со истите. Корисникот може да листа низ картичките и бира за кој часови ќе прегледува. 
Користеното API е [[OpenWeatherMap](https://openweathermap.org/)]. Поради ограничувања од бесплатната верзија податоците се добиваат за секој следни 3 часа.

## Опис на решението
Апликацијата е имплементирана така се внесува име на градот , потоа за тоа име се праќа API повик , се добиваат информации за дадениот град и тие информации од json се конвертираат во потребниот формат и се чуваат во еден DataTable . Исто така потребните слики и добиениот json се чуваат во АppData директориумот на User-от. Се користат и 2 Frameworks ( MetroUI и Newtonsoft Json ) кој служат за подобар и по модерен изглед , a другиот се користи за серијализација и десеријализација на json string. Имплементирана е и серијализација да се зачувува последниот пребаруван град кога повторно ќе се вклучи апликацијата.
<br>
#### 1.Податоци кој се чуваат ( во User/AppData/Roaming/Weather_App)<br>
+ Json String од последниот пребаруван град<br>
+ Потребни слики кој се добиваат од API<br>
#### 2.Структури и класи <br>
+ WeatherForecast – класа која ја прави HTTP поврзаноста кон OpenWeatherAPI и враќа објект од тип Rootobject<br>
  + Rootobject – ги содржи податоците за градот (City city) и листата на временски мерења (List[] list)<br>
  + City – информации за градот (име, координати, sunrise/sunset)<br>
  + List – секој запис во листата содржи Main (температура), Weather (опис и икона), Clouds, Wind ... <br>
+ DataTable – во Form1 се креира DataTable каде што секој ред претставува еден временски запис, со колони:
"Temp", "Temp Min", "Temp Max", "Clouds", "Humidity", "Weather Description", "Icon", "Location Name", "Sunrise", "Sunset", "Wind Speed m/s", "Date", "Day of week", "Time", "Wind Direction" итн <br>
+ ImageConverter – се користи за претворање на сликите во byte[] и обратно за прикажување во PictureBox контролите <br>
#### 3.Чување и поврат на податоци<br>
Кога корисникот пребарува град:
+ Апликацијата повикува WeatherForecast.getApiData(cityName)<br>
+ JSON одговорот се користи за креирање на DataTable<br>
+ Иконите се симнуваат и конвертираат во byte[] за прикажување<br>
+ Податоците се прикажуваат на tiles и графикон<br>
+ Последниот внесен град се зачувува во appStateFile<br>
При следно отворање:
+ Се проверува appStateFile, ако постои се чита градот и се прикажува временската прогноза автоматски<br>
+ JSON може да се десериализира во Rootobject и да се користи за повторно креирање на DataTable<br>
## Опис на функции и класи
### Функција: rotateImage
```
private Image rotateImage(Image windDirectionArrow, float angle)
{
    if (windDirectionArrow == null)
        throw new ArgumentNullException(nameof(windDirectionArrow));

    float adjustedAngle = angle - 90f;
    adjustedAngle += 180f;

    Bitmap rotatedBmp = new Bitmap(windDirectionArrow.Width, windDirectionArrow.Height);
    rotatedBmp.SetResolution(windDirectionArrow.HorizontalResolution, windDirectionArrow.VerticalResolution);
    using (Graphics g = Graphics.FromImage(rotatedBmp))
    {
        g.TranslateTransform(windDirectionArrow.Width / 2f, windDirectionArrow.Height / 2f);
        g.RotateTransform(adjustedAngle);
        g.TranslateTransform(-windDirectionArrow.Width / 2f, -windDirectionArrow.Height / 2f);
        g.DrawImage(windDirectionArrow, new Point(0, 0));
    }
    return rotatedBmp;
}
```
Оваа функција служи за ротација на стрелката ( слика ) која покажува во која насока се дува ветерот. Ротацијата се врши според аголот кој се добива од API-то кој потоа се внесува како влезен параметар во функцијата.
Логика на функцијата : <br>
+ Се проверува дали сликата е null
+ Се прилагодува аголот (adjustedAngle) со формула angle - 90 + 180. Ова се прави поради тоа што стрелката првично е свртена на десно.
+ Се креира нов Bitmap со иста големина како оригиналната слика
+ Со Graphics објект се поставува центарот на сликата како точка за ротација (TranslateTransform)
+ Се врши ротацијата со RotateTransform(adjustedAngle)
+ На крај се исцртува оригиналната слика врз новата ротирана и се враќа како излез од функцијата.

## Генеративна вештачка интелигенција
Генеративната вештачка интелигенција беше користена како еден вид на асистен во развој на апликацијата.Користен модел е GPT-5.0o . Многу често е користена за поставување на некој обични парашања од типот зошто ова не работи, зошто се јавува оваа грешка итн. Исто така таа е користена за генерирање на помошни функции како на пример функцијата private DateTime convertUnixToDT(double unixTime). Prompt кој е користен за генерирање на оваа функција е: <br>
+ Write me a function that converts UNIX to standart DateTime , input is double ut.<br>

Исто така користена е за помош зошто packages кој се користени не се loadiraat кај сите. Користен prompt е:<br>
+ Why dosene't Visual Studio restore the packages automatically after loading?

## WeatherApp
MK / **EN** <br>
<br>
The application is a basic implementation of a typical weather forecast app. It has a very simple way of use that any user can easily understand. It contains cards displaying weather information for a given hour, as well as a chart showing temperatures for specific hours.
