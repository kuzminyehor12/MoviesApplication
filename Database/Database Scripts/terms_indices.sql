CREATE INDEX IF NOT EXISTS IX_Terms_TermText
    ON "Terms" ("TermText");

CREATE INDEX IF NOT EXISTS IX_Terms_TermType
    ON "Terms" ("TermType");

CREATE INDEX IF NOT EXISTS IX_TermIndex_MovieTerm ON "TermIndex" ("MovieId", "TermId");

CREATE INDEX IF NOT EXISTS IX_TermVectors_MovieTerm ON "TermVectors" ("MovieId", "TermId");