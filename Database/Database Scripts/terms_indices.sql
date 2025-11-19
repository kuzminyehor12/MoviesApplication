-- 1. Index on TermText (Standard B-tree Index)
CREATE INDEX IF NOT EXISTS IX_Terms_TermText
    ON "Terms" ("TermText");

-- 2. Index on TermType (Standard B-tree Index)
CREATE INDEX IF NOT EXISTS IX_Terms_TermType
    ON "Terms" ("TermType");

CREATE INDEX IF NOT EXISTS IX_TermIndex_MovieTerm ON "TermIndex" ("MovieId", "TermId");

CREATE INDEX IF NOT EXISTS IX_TermVectors_MovieTerm ON "TermVector" ("MovieId", "TermId");