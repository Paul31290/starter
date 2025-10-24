import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface Language {
  code: string;
  name: string;
  flag: string;
}

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  private currentLanguageSubject = new BehaviorSubject<string>('en');
  public currentLanguage$: Observable<string> = this.currentLanguageSubject.asObservable();

  private supportedLanguages: Language[] = [
    { code: 'en', name: 'English', flag: '🇺🇸' },
    { code: 'es', name: 'Español', flag: '🇪🇸' },
    { code: 'fr', name: 'Français', flag: '🇫🇷' }
  ];

  constructor() {
    const browserLang = navigator.language.split('-')[0];
    const defaultLang = this.supportedLanguages.find(lang => lang.code === browserLang) ? browserLang : 'en';
    this.setLanguage(localStorage.getItem('language') || defaultLang);
  }

  getCurrentLanguage(): string {
    return this.currentLanguageSubject.value;
  }

  setLanguage(languageCode: string): void {
    if (this.supportedLanguages.find(lang => lang.code === languageCode)) {
      this.currentLanguageSubject.next(languageCode);
      localStorage.setItem('language', languageCode);
    }
  }

  getSupportedLanguages(): Language[] {
    return [...this.supportedLanguages];
  }

  translate(key: string): string {
    const translations: { [key: string]: { [lang: string]: string } } = {
      'SETTINGS.LANGUAGE': {
        'en': 'Language',
        'es': 'Idioma',
        'fr': 'Langue'
      },
      'COMMON.SAVE': {
        'en': 'Save',
        'es': 'Guardar',
        'fr': 'Enregistrer'
      },
      'COMMON.CANCEL': {
        'en': 'Cancel',
        'es': 'Cancelar',
        'fr': 'Annuler'
      }
    };

    const currentLang = this.getCurrentLanguage();
    return translations[key]?.[currentLang] || key;
  }
} 