

export interface ICoursesWithDetailDto {
    courseId: number;
    name: string;
    description: string;
    price: number;
    about: string;
    thumbnailUrl: string;
    videosCount: number;
    studentsCount: number;
    status: string;
    level: string;
    language: string;
    instructor: string;
    profilePictureUrl: string;
    studentsCountForInstructor: number;
    coursesCountForInstructor: number;
}
