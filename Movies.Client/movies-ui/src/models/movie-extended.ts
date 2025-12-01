import { CastMember } from "./cast-member";
import { CrewMember } from "./crew-member";
import { Keyword } from "./keyword";
import { Movie } from "./movie";

export class MovieExtended extends Movie {
    overview?: string;
    tagLine?: string;
    keywords: Keyword[] = [];
    crewMembers: CrewMember[] = [];
    castMembers: CastMember[] = [];
}