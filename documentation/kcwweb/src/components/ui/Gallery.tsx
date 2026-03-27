import { useState } from 'react';
import type { ConceptArt, ConceptArtCategory, GalleryProps } from '../../types';
import { Card, CardBody, CardTitle, CardDescription } from './Card';
import { Modal } from './Modal';
import { Button } from './Button';
import './Gallery.css';

const categoryLabels: Record<ConceptArtCategory, string> = {
  characters: 'Персонажи',
  environments: 'Окружение',
  vehicles: 'Транспорт',
  weapons: 'Оружие',
  ui: 'Интерфейс',
  other: 'Другое',
};

const allCategories: ConceptArtCategory[] = [
  'characters',
  'environments',
  'vehicles',
  'weapons',
  'ui',
  'other',
];

export function Gallery({ items, title, filterByCategory }: GalleryProps) {
  const [selectedCategory, setSelectedCategory] = useState<ConceptArtCategory | 'all'>('all');
  const [selectedImage, setSelectedImage] = useState<ConceptArt | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const filteredItems = items.filter((item) => {
    const matchesCategory = filterByCategory
      ? item.category === filterByCategory
      : selectedCategory === 'all' || item.category === selectedCategory;
    return matchesCategory;
  });

  const handleImageClick = (item: ConceptArt) => {
    setSelectedImage(item);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setTimeout(() => setSelectedImage(null), 300);
  };

  const currentIndex = selectedImage ? items.findIndex((item) => item.id === selectedImage.id) : -1;

  const handlePrevious = () => {
    if (currentIndex > 0) {
      setSelectedImage(items[currentIndex - 1]);
    } else if (items.length > 0) {
      setSelectedImage(items[items.length - 1]);
    }
  };

  const handleNext = () => {
    if (currentIndex < items.length - 1) {
      setSelectedImage(items[currentIndex + 1]);
    } else if (items.length > 0) {
      setSelectedImage(items[0]);
    }
  };

  return (
    <section className="gallery">
      {title && <h2 className="gallery-title">{title}</h2>}

      {!filterByCategory && (
        <div className="gallery-filters">
          <Button
            variant={selectedCategory === 'all' ? 'primary' : 'outline'}
            size="sm"
            onClick={() => setSelectedCategory('all')}
          >
            Все
          </Button>
          {allCategories.map((category) => (
            <Button
              key={category}
              variant={selectedCategory === category ? 'primary' : 'outline'}
              size="sm"
              onClick={() => setSelectedCategory(category)}
            >
              {categoryLabels[category]}
            </Button>
          ))}
        </div>
      )}

      <div className="gallery-grid">
        {filteredItems.map((item) => (
          <Card
            key={item.id}
            className="gallery-item"
            hoverable
            onClick={() => handleImageClick(item)}
          >
            <div className="gallery-image-wrapper">
              <img
                src={item.imageUrl}
                alt={item.title}
                className="gallery-image"
                loading="lazy"
              />
              <div className="gallery-overlay">
                <span className="gallery-overlay-text">Просмотреть</span>
              </div>
            </div>
            <CardBody>
              <CardTitle>{item.title}</CardTitle>
              <CardDescription>{item.description}</CardDescription>
              <div className="gallery-tags">
                {item.tags.slice(0, 3).map((tag) => (
                  <span key={tag} className="gallery-tag">
                    {tag}
                  </span>
                ))}
              </div>
            </CardBody>
          </Card>
        ))}
      </div>

      {filteredItems.length === 0 && (
        <p className="gallery-empty">В этой категории пока нет работ</p>
      )}

      <Modal isOpen={isModalOpen} onClose={handleCloseModal}>
        {selectedImage && (
          <div className="lightbox">
            <button className="lightbox-nav lightbox-nav-prev" onClick={handlePrevious} aria-label="Previous image">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M15 18l-6-6 6-6" />
              </svg>
            </button>
            <img src={selectedImage.imageUrl} alt={selectedImage.title} className="lightbox-image" />
            <button className="lightbox-nav lightbox-nav-next" onClick={handleNext} aria-label="Next image">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M9 18l6-6-6-6" />
              </svg>
            </button>
            <div className="lightbox-info">
              <h3 className="lightbox-title">{selectedImage.title}</h3>
              <p className="lightbox-description">{selectedImage.description}</p>
              <div className="lightbox-meta">
                <span className="lightbox-category">{categoryLabels[selectedImage.category]}</span>
                <div className="lightbox-tags">
                  {selectedImage.tags.map((tag) => (
                    <span key={tag} className="lightbox-tag">#{tag}</span>
                  ))}
                </div>
              </div>
            </div>
          </div>
        )}
      </Modal>
    </section>
  );
}
