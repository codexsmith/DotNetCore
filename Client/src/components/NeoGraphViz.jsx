import React, { useEffect, useRef } from "react";
import NeoVis from "neovis.js";

const NeoGraphViz = () => {
  const visRef = useRef(null);

  useEffect(() => {
    // Neo4j configuration
    const config = {
      container_id: visRef.current,
      server_url: "bolt+ssc://localhost:7687", // Update with your Neo4j instance URL
      server_user: "neo4j",               // Update with your username
      server_password: "Dream123191",    // Update with your password
      labels: {
        User: {
          caption: "UserName",
        },
        Note: {
          caption: "Title",
        },
      },
      relationships: {
        TAGGED_WITH: {
          caption: true,
        },
      },
      console_debug: true,        // Enable detailed logs
    };

    // Initialize Neovis.js
    const viz = new NeoVis(config);
    viz.render();

    // Cleanup on component unmount
    return () => {
      viz.clearNetwork();
    };
  }, []);

  return <div ref={visRef} style={{ width: "100%", height: "500px" }} />;
};

export default NeoGraphViz;
