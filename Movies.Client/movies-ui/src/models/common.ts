class Entity {
    id!: number;
}

class NamedEntity extends Entity {
    name!: string;
}

type NotAssigned = "N/A";