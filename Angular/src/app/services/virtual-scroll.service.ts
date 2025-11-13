import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, combineLatest } from 'rxjs';
import { map, distinctUntilChanged } from 'rxjs/operators';

export interface VirtualScrollConfig {
  itemSize: number;
  bufferSize: number;
  containerHeight: number;
  overscan?: number;
}

export interface VirtualScrollState {
  startIndex: number;
  endIndex: number;
  visibleItems: any[];
  totalHeight: number;
  scrollTop: number;
  containerHeight: number;
}

@Injectable({
  providedIn: 'root'
})
export class VirtualScrollService {
  private scrollTopSubject = new BehaviorSubject<number>(0);
  private containerHeightSubject = new BehaviorSubject<number>(0);
  private itemsSubject = new BehaviorSubject<any[]>([]);
  private configSubject = new BehaviorSubject<VirtualScrollConfig>({
    itemSize: 50,
    bufferSize: 5,
    containerHeight: 400,
    overscan: 3
  });

  scrollTop$ = this.scrollTopSubject.asObservable();
  containerHeight$ = this.containerHeightSubject.asObservable();
  items$ = this.itemsSubject.asObservable();
  config$ = this.configSubject.asObservable();

  virtualScrollState$: Observable<VirtualScrollState> = combineLatest([
    this.scrollTop$,
    this.containerHeight$,
    this.items$,
    this.config$
  ]).pipe(
    map(([scrollTop, containerHeight, items, config]) => {
      return this.calculateVirtualScrollState(scrollTop, containerHeight, items, config);
    }),
    distinctUntilChanged((prev, curr) => 
      prev.startIndex === curr.startIndex && 
      prev.endIndex === curr.endIndex &&
      prev.scrollTop === curr.scrollTop
    )
  );

  updateScrollTop(scrollTop: number): void {
    this.scrollTopSubject.next(scrollTop);
  }

  updateContainerHeight(height: number): void {
    this.containerHeightSubject.next(height);
  }

  updateItems(items: any[]): void {
    this.itemsSubject.next(items);
  }

  updateConfig(config: Partial<VirtualScrollConfig>): void {
    const currentConfig = this.configSubject.value;
    this.configSubject.next({ ...currentConfig, ...config });
  }

  private calculateVirtualScrollState(
    scrollTop: number,
    containerHeight: number,
    items: any[],
    config: VirtualScrollConfig
  ): VirtualScrollState {
    const { itemSize, bufferSize, overscan = 3 } = config;
    const totalHeight = items.length * itemSize;

    const startIndex = Math.max(0, Math.floor(scrollTop / itemSize) - overscan);
    const visibleCount = Math.ceil(containerHeight / itemSize) + (overscan * 2);
    const endIndex = Math.min(items.length - 1, startIndex + visibleCount);

    const visibleItems = items.slice(startIndex, endIndex + 1);

    return {
      startIndex,
      endIndex,
      visibleItems,
      totalHeight,
      scrollTop,
      containerHeight
    };
  }

  getItemOffset(index: number, config: VirtualScrollConfig): number {
    return index * config.itemSize;
  }

  getScrollTopForIndex(index: number, config: VirtualScrollConfig): number {
    return index * config.itemSize;
  }

  getIndexFromScrollTop(scrollTop: number, config: VirtualScrollConfig): number {
    return Math.floor(scrollTop / config.itemSize);
  }

  calculateOptimalBufferSize(containerHeight: number, itemSize: number): number {
    const visibleItems = Math.ceil(containerHeight / itemSize);
    return Math.max(5, Math.ceil(visibleItems * 0.5));
  }

  scrollToItem(index: number, config: VirtualScrollConfig, behavior: ScrollBehavior = 'smooth'): void {
    const scrollTop = this.getScrollTopForIndex(index, config);
    this.updateScrollTop(scrollTop);
  }

  scrollToTop(behavior: ScrollBehavior = 'smooth'): void {
    this.updateScrollTop(0);
  }

  scrollToBottom(config: VirtualScrollConfig, behavior: ScrollBehavior = 'smooth'): void {
    const items = this.itemsSubject.value;
    const scrollTop = this.getScrollTopForIndex(items.length - 1, config);
    this.updateScrollTop(scrollTop);
  }

  isItemVisible(index: number, state: VirtualScrollState): boolean {
    return index >= state.startIndex && index <= state.endIndex;
  }

  getVisibleRangeInfo(state: VirtualScrollState): {
    visibleStart: number;
    visibleEnd: number;
    totalItems: number;
    visibleCount: number;
  } {
    const items = this.itemsSubject.value;
    return {
      visibleStart: state.startIndex,
      visibleEnd: state.endIndex,
      totalItems: items.length,
      visibleCount: state.visibleItems.length
    };
  }

  getPerformanceMetrics(state: VirtualScrollState): {
    renderEfficiency: number;
    memoryUsage: number;
    scrollPerformance: number;
  } {
    const items = this.itemsSubject.value;
    const renderEfficiency = (state.visibleItems.length / items.length) * 100;
    
    const memoryUsage = state.visibleItems.length * 1024;
    
    const scrollPerformance = state.visibleItems.length;

    return {
      renderEfficiency,
      memoryUsage,
      scrollPerformance
    };
  }

  destroy(): void {
    this.scrollTopSubject.complete();
    this.containerHeightSubject.complete();
    this.itemsSubject.complete();
    this.configSubject.complete();
  }
}
