// This Cypher script creates the following graph structure
// (User)-[:OWNS]->(Project)
// (Project)-[:HAS_NOTE]->(Note)-[:HAS_TAG]->(Tag)

// Create a user
CREATE (u:User {id: '1', email: 'nsc319@gmail', name: 'Nick'})
WITH u

// Create a project owned by the user
CREATE (p:Project {id: '101', name: 'Sample Project'})
MERGE (u)-[:OWNS]->(p)
WITH p, u

// Create two notes under the project
CREATE (n1:Note {id: '201', content: 'This is the first note', createdAt: datetime()})
CREATE (n2:Note {id: '202', content: 'This is the second note', createdAt: datetime()})
MERGE (p)-[:HAS_NOTE]->(n1)
MERGE (p)-[:HAS_NOTE]->(n2)
WITH n1, n2

// Create tags for the first note
CREATE (t1a:Tag {id: '301', name: 'Important'})
CREATE (t1b:Tag {id: '302', name: 'Urgent'})
MERGE (n1)-[:HAS_TAG]->(t1a)
MERGE (n1)-[:HAS_TAG]->(t1b)
WITH n2

// Create tags for the second note
CREATE (t2a:Tag {id: '303', name: 'Meeting'})
CREATE (t2b:Tag {id: '304', name: 'Follow-Up'})
MERGE (n2)-[:HAS_TAG]->(t2a)
MERGE (n2)-[:HAS_TAG]->(t2b)
RETURN 'Seeding complete' AS Result;
