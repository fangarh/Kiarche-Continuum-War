import { Link } from 'react-router-dom';
import './Footer.css';

export function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="footer">
      <div className="footer-container">
        <div className="footer-section">
          <div className="footer-logo">
            <span className="logo-icon">◈</span>
            <span className="logo-text">Kiarche Continuum War</span>
          </div>
          <p className="footer-tagline">
            Исследуй бесконечность. Открой тайны древних.
          </p>
        </div>

        <div className="footer-section">
          <h4 className="footer-title">Навигация</h4>
          <ul className="footer-links">
            <li><Link to="/">Главная</Link></li>
            <li><Link to="/overview">Об игре</Link></li>
            <li><Link to="/lore">Лор</Link></li>
            <li><Link to="/mechanics">Механики</Link></li>
            <li><Link to="/roadmap">Roadmap</Link></li>
          </ul>
        </div>

        <div className="footer-section">
          <h4 className="footer-title">Разработка</h4>
          <ul className="footer-links">
            <li><Link to="/architecture">Архитектура</Link></li>
            <li><Link to="/docs">Документация</Link></li>
            <li><Link to="/devlog">Devlog</Link></li>
            <li><Link to="/team">Команда</Link></li>
          </ul>
        </div>

        <div className="footer-section">
          <h4 className="footer-title">Следите за нами</h4>
          <div className="footer-social">
            <a href="#" className="social-link" aria-label="GitHub">
              <svg viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 0C5.37 0 0 5.37 0 12c0 5.31 3.435 9.795 8.205 11.385.6.105.825-.255.825-.57 0-.285-.015-1.23-.015-2.235-3.015.555-3.795-.735-4.035-1.41-.135-.345-.72-1.41-1.23-1.695-.42-.225-1.02-.78-.015-.795.945-.015 1.62.87 1.845 1.23 1.08 1.815 2.805 1.305 3.495.99.105-.78.42-1.305.765-1.605-2.67-.3-5.46-1.335-5.46-5.925 0-1.305.465-2.385 1.23-3.225-.12-.3-.54-1.53.12-3.18 0 0 1.005-.315 3.3 1.23.96-.27 1.98-.405 3-.405s2.04.135 3 .405c2.295-1.56 3.3-1.23 3.3-1.23.66 1.65.24 2.88.12 3.18.765.84 1.23 1.905 1.23 3.225 0 4.605-2.805 5.625-5.475 5.925.435.375.81 1.095.81 2.22 0 1.605-.015 2.895-.015 3.3 0 .315.225.69.825.57A12.02 12.02 0 0024 12c0-6.63-5.37-12-12-12z"/>
              </svg>
            </a>
            <a href="#" className="social-link" aria-label="Twitter">
              <svg viewBox="0 0 24 24" fill="currentColor">
                <path d="M18.244 2.25h3.308l-7.227 8.26 8.502 11.24H16.17l-5.214-6.817L4.99 21.75H1.68l7.73-8.835L1.254 2.25H8.08l4.713 6.231zm-1.161 17.52h1.833L7.084 4.126H5.117z"/>
              </svg>
            </a>
            <a href="#" className="social-link" aria-label="Discord">
              <svg viewBox="0 0 24 24" fill="currentColor">
                <path d="M20.317 4.37a19.791 19.791 0 00-4.885-1.515.074.074 0 00-.079.037c-.21.375-.444.864-.608 1.25a18.27 18.27 0 00-5.487 0 12.64 12.64 0 00-.617-1.25.077.077 0 00-.079-.037A19.736 19.736 0 003.677 4.37a.07.07 0 00-.032.027C.533 9.046-.32 13.58.099 18.057a.082.082 0 00.031.057 19.9 19.9 0 005.993 3.03.078.078 0 00.084-.028c.462-.63.874-1.295 1.226-1.994a.076.076 0 00-.041-.106 13.107 13.107 0 01-1.872-.892.077.077 0 01-.008-.128 10.2 10.2 0 00.372-.292.074.074 0 01.077-.01c3.928 1.793 8.18 1.793 12.062 0a.074.074 0 01.078.01c.12.098.246.198.373.292a.077.077 0 01-.006.127 12.299 12.299 0 01-1.873.892.077.077 0 00-.041.107c.36.698.772 1.362 1.225 1.993a.076.076 0 00.084.028 19.839 19.839 0 006.002-3.03.077.077 0 00.032-.054c.5-5.177-.838-9.674-3.549-13.66a.061.061 0 00-.031-.03zM8.02 15.33c-1.183 0-2.157-1.085-2.157-2.419 0-1.333.956-2.419 2.157-2.419 1.21 0 2.176 1.096 2.157 2.42 0 1.333-.956 2.418-2.157 2.418zm7.975 0c-1.183 0-2.157-1.085-2.157-2.419 0-1.333.955-2.419 2.157-2.419 1.21 0 2.176 1.096 2.157 2.42 0 1.333-.946 2.418-2.157 2.418z"/>
              </svg>
            </a>
            <a href="#" className="social-link" aria-label="YouTube">
              <svg viewBox="0 0 24 24" fill="currentColor">
                <path d="M23.498 6.186a3.016 3.016 0 00-2.122-2.136C19.505 3.545 12 3.545 12 3.545s-7.505 0-9.377.505A3.017 3.017 0 00.502 6.186C0 8.07 0 12 0 12s0 3.93.502 5.814a3.016 3.016 0 002.122 2.136c1.871.505 9.376.505 9.376.505s7.505 0 9.377-.505a3.015 3.015 0 002.122-2.136C24 15.93 24 12 24 12s0-3.93-.502-5.814zM9.545 15.568V8.432L15.818 12l-6.273 3.568z"/>
              </svg>
            </a>
          </div>
        </div>

        <div className="footer-section footer-steam">
          <a href="https://store.steampowered.com/app/placeholder" className="steam-button" target="_blank" rel="noopener noreferrer">
            <svg viewBox="0 0 24 24" fill="currentColor" className="steam-icon">
              <path d="M11.979 0C5.678 0 .511 4.86.022 11.037l4.455 1.841a3.001 3.001 0 012.822-1.841c.431 0 .837.115 1.192.316l2.892-4.185V7.1c0-.295.023-.584.066-.868L6.606 4.28A7.037 7.037 0 0111.979 2c3.896 0 7.037 3.14 7.037 7.037 0 1.218-.31 2.362-.853 3.363l4.682 2.81A11.935 11.935 0 0024 11.979C24 5.365 18.635 0 11.979 0zM7.29 13.004a1.001 1.001 0 100 2.002 1.001 1.001 0 000-2.002zm5.99-3.996a3.99 3.99 0 100 7.98 3.99 3.99 0 000-7.98zm4.49 6.493l-2.42-1.452a3.97 3.97 0 00.93-2.54c0-.363-.05-.714-.14-1.05l2.92 1.75a7.007 7.007 0 01-1.29 3.292z"/>
            </svg>
            <span>Добавить в Wishlist</span>
          </a>
        </div>
      </div>

      <div className="footer-bottom">
        <p>&copy; {currentYear} Kiarche Continuum War. Все права защищены.</p>
        <p className="footer-disclaimer">
          Это документация проекта. Игра находится в разработке.
        </p>
      </div>
    </footer>
  );
}
