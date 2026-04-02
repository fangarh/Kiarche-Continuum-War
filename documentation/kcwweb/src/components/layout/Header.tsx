import { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { navigation } from '../../data';
import './Header.css';

export function Header() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const location = useLocation();

  const toggleMenu = () => setIsMenuOpen(!isMenuOpen);
  const closeMenu = () => setIsMenuOpen(false);

  return (
    <header className="header">
      <div className="header-container">
        <Link to="/" className="logo" onClick={closeMenu}>
          <span className="logo-icon">◈</span>
          <div className="logo-text-wrapper">
            <span className="logo-text-main">Kiarche</span>
            <span className="logo-text-sub">Continuum War</span>
          </div>
        </Link>

        {/* Desktop Navigation */}
        <nav className="nav desktop-nav">
          <ul className="nav-list">
            {navigation.map((item) => (
              <li key={item.path} className="nav-item">
                <Link
                  to={item.path}
                  className={`nav-link ${location.pathname === item.path ? 'nav-link-active' : ''}`}
                >
                  {item.label}
                </Link>
              </li>
            ))}
          </ul>
        </nav>

        <button
          className={`menu-toggle ${isMenuOpen ? 'is-active' : ''}`}
          onClick={toggleMenu}
          aria-label="Toggle menu"
          aria-expanded={isMenuOpen}
        >
          <span className="menu-toggle-line"></span>
          <span className="menu-toggle-line"></span>
          <span className="menu-toggle-line"></span>
        </button>
      </div>

      {/* Mobile Navigation - Separate element for total control */}
      <nav className={`mobile-nav ${isMenuOpen ? 'mobile-nav-open' : ''}`}>
        <ul className="mobile-nav-list">
          {navigation.map((item) => (
            <li key={item.path} className="mobile-nav-item">
              <Link
                to={item.path}
                className={`mobile-nav-link ${location.pathname === item.path ? 'nav-link-active' : ''}`}
                onClick={closeMenu}
              >
                {item.label}
              </Link>
            </li>
          ))}
        </ul>
      </nav>
    </header>
  );
}
