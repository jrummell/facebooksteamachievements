import IAchievement from "./IAchievement";
import IGame from "./IGame";

export default interface IGameAchievements {
    game: IGame;
    achievements: IAchievement[];
}
