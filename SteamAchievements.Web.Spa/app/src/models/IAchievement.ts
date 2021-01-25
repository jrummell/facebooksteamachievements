export default interface IAchievement {
    id: number;
    apiName: string;
    imageUrl: string;
    name: string;
    description: string;
    language: string;
    selected?: boolean;
}
