-- Synsets
CREATE TABLE Synsets
(
	SynsetId	INT PRIMARY KEY,
	WordNetId	INT UNIQUE NOT NULL,
	SynsetType	INT NOT NULL,
	Gloss		NVARCHAR(4000)
);

CREATE INDEX IX_Synsets_SynsetId ON Synsets (SynsetId);
CREATE INDEX IX_Synsets_WordNetId ON Synsets (WordNetId);

-- Word Senses
CREATE TABLE WordSenses
(
	WordSenseId	INT PRIMARY KEY,
	
	SynsetId	INT REFERENCES Synsets(SynsetId),
	WordNum		INT NOT NULL,
	
	Lemma		NVARCHAR(1000) NOT NULL,
	SenseNumber	INT NOT NULL,
	TagCount	INT,
	SenseKey	NVARCHAR,
	Syntax		INT,
	Frame		INT	
);

CREATE INDEX IX_WordSenses_Synset ON WordSenses (SynsetId, WordNum);
CREATE INDEX IX_WordSenses_Lemma ON WordSenses (Lemma);

-- Semantic Classes
CREATE TABLE SemanticClasses
(
	SemanticClassId	INT PRIMARY KEY,
	ClassType		INT NOT NULL
);

CREATE TABLE SemanticClassHeads
(
	SemanticClassId	INT REFERENCES SemanticClasses(SemanticClassId),
	WordSenseId		INT REFERENCES WordSenses(WordSenseId)
);

CREATE TABLE SemanticClassMembers
(
	SemanticClassId	INT REFERENCES SemanticClasses(SemanticClassId),
	WordSenseId		INT REFERENCES WordSenses(WordSenseId)
);

-- Groups
CREATE TABLE Groups
(
	FirstWordSenseId	INT REFERENCES WordSenses(WordSenseId),
	SecondWordSenseId	INT REFERENCES WordSenses(WordSenseId)
);

-- Antonyms
CREATE TABLE Antonyms
(
	FirstWordSenseId	INT REFERENCES WordSenses(WordSenseId),
	SecondWordSenseId	INT REFERENCES WordSenses(WordSenseId)
);

-- See Also
CREATE TABLE SeeAlsos
(
	FirstWordSenseId	INT REFERENCES WordSenses(WordSenseId),
	SecondWordSenseId	INT REFERENCES WordSenses(WordSenseId)
);

-- Participles
CREATE TABLE Participles
(
	FirstWordSenseId	INT REFERENCES WordSenses(WordSenseId),
	SecondWordSenseId	INT REFERENCES WordSenses(WordSenseId)
);

-- PertainsTo
CREATE TABLE PertainsTo
(
	FirstWordSenseId	INT REFERENCES WordSenses(WordSenseId),
	SecondWordSenseId	INT REFERENCES WordSenses(WordSenseId)
);

-- Hypernyms
CREATE TABLE Hypernyms
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

CREATE TABLE Hyponyms
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

-- Prototype/Instance
CREATE TABLE Prototypes
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

CREATE TABLE Instances
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

-- Entailments
CREATE TABLE Entailments
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

-- Satellites
CREATE TABLE Satellites
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

-- Member
CREATE TABLE MemberMeronyms
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

CREATE TABLE MemberHolonyms
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

-- Substance
CREATE TABLE SubstanceMeronyms
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

CREATE TABLE SubstanceHolonyms
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

-- Part
CREATE TABLE PartMeronyms
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

CREATE TABLE PartHolonyms
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

-- Derivations
CREATE TABLE Derivations
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

-- Causes
CREATE TABLE Causes
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);

-- Attributes
CREATE TABLE Attributes
(
	FirstSynsetId	INT REFERENCES Synsets(SynsetId),
	SecondSynsetId	INT REFERENCES Synsets(SynsetId)
);
