import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Layout } from './components/layout';
import {
  HomePage,
  OverviewPage,
  LorePage,
  MechanicsPage,
  ArchitecturePage,
  ConventionsPage,
  DocsPage,
  RoadmapPage,
  DevlogPage,
  TeamPage,
} from './pages';
import './styles/global.css';

function App() {
  return (
    <Router>
      <Layout>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/overview" element={<OverviewPage />} />
          <Route path="/lore" element={<LorePage />} />
          <Route path="/mechanics" element={<MechanicsPage />} />
          <Route path="/architecture" element={<ArchitecturePage />} />
          <Route path="/conventions" element={<ConventionsPage />} />
          <Route path="/docs" element={<DocsPage />} />
          <Route path="/roadmap" element={<RoadmapPage />} />
          <Route path="/devlog" element={<DevlogPage />} />
          <Route path="/team" element={<TeamPage />} />
        </Routes>
      </Layout>
    </Router>
  );
}

export default App;
