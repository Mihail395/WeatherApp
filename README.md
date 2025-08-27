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
