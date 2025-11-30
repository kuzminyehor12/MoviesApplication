class MovieExtended extends Movie {
    overview?: string;
    tagLine?: string;
    keywords: Keyword[] = [];
    crewMembers: CrewMember[] = [];
    castMembers: CastMember[] = [];
}