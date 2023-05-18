export interface ICourseForInstructorDto {
    courseId: number;
    name: string;
    about: string;
    description: string;
    price: number;
    level: string;
    language: string;
    thumbnailUrl: string;
    category: string;
    videosCount: number;
    studentsCount: number;
    isDelete: boolean;
}
