# IMPORTANT READ FIRST!
**По вклучувуње во Visual Studio, доколку се појави Error кој што покажува дека некој Packages не се инсталирани ве молам притиснете десен клик на Solution во Solution Explorer-от и на Restore NuGet Packages. Кај некој се случува проблем Visual Studio да не ги врати и вчита сам потребните пакети од Frameworks кој се користат во апликацијата иако во packages config files и .csproject тие се наведени како top level dependencies.** / **Regarding Visual Studio, if an error appears indicating that some packages are not installed, please right-click on the Solution in the Solution Explorer and select Restore NuGet Packages. For some users, there is an issue where Visual Studio does not automatically restore and load the required packages from the frameworks used in the application, even though they are listed as top-level dependencies in the packages.config files and the .csproj files.**

# WeatherApp
**MK** / EN
Апликацијата е basic имплементација на типична апликација за временска прогноза. Има многу едноставен начин на користење кoј што секој корисник може многу лесно да го разбере. Содржи картички на кој се гледаат информации за времето во даден час , исто така и график на температурите во дадени часови.

## Користење
Апликацијата се користи многу едноставно така што при стартување се вклучува прозорецот на кој само се гледаат полето каде што треба да се внесе името на градот и едно копче за барање (Search). Го внесувате името на градот за кој сакате да пребарате и потоа притискате на копчето и чекате. 
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
